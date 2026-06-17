using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using tara_tool.Data.Tables;

namespace tara_tool.Data.Services;

public class ItemDefinitionService(
    IDbContextFactory<ApplicationDbContext> contextFactory,
    AccessControlService accessControlService)
    : IDataService<ItemDefinition>
{
    public async Task<ItemDefinition?> CreateItemAsync(long projectID,
                                                       string name)
    {
        using ApplicationDbContext context =
            await contextFactory.CreateDbContextAsync();
        if (await accessControlService.CheckUserAccessRightsWrite(projectID) ==
            false)
        {
            return null;
        }

        long nextItemNumber =
            await context.ItemDefinitions.Where(item => item.IdProject == projectID)
                .Select(item => (long?)item.ItemNumber)
                .MaxAsync() ??
            0;

        ItemDefinition newItem = new ItemDefinition
        {
            ItemNumber = nextItemNumber + 1,
            ItemName = name,
            Project = await context.Projects.FindAsync(projectID) ??
                    throw new Exception("Invalid Project ID for Item Creation")
        };

        context.ItemDefinitions.Add(newItem);
        newItem.Project.DateLastChanged = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return newItem;
    }

    public async Task<ItemDefinition?> GetItemByIdAsync(
        long id,
        Func<IQueryable<ItemDefinition>, IQueryable<ItemDefinition>>? include =
            null,
        CancellationToken cancellationToken = default)
    {
        using ApplicationDbContext context =
            await contextFactory.CreateDbContextAsync();
        IQueryable<ItemDefinition> itemDefinitions =
            context.ItemDefinitions.AsNoTracking().AsQueryable();
        if (include is not null)
        {
            itemDefinitions = include(itemDefinitions);
        }
        ItemDefinition? itemDefinition =
            await itemDefinitions.FirstOrDefaultAsync(i => i.Id == id);
        if (itemDefinition is null)
        {
            return null;
        }
        if (await accessControlService.CheckUserAccessRightsRead(
                itemDefinition.IdProject) is false)
        {
            return null;
        }

        return itemDefinition;
    }

    public async Task<List<ItemDefinition>> GetItemsAsync(
        long projectID,
        Func<DbSet<ItemDefinition>, IQueryable<ItemDefinition>>? extend = null)
    {
        if (await accessControlService.CheckUserAccessRightsRead(projectID)
                is false)
        {
            return new List<ItemDefinition>();
        }

        using ApplicationDbContext context =
            await contextFactory.CreateDbContextAsync();
        DbSet<ItemDefinition> itemSet = context.ItemDefinitions;
        IQueryable<ItemDefinition> itemQuery = itemSet.AsQueryable();
        if (extend != null)
        {
            itemQuery = extend.Invoke(itemSet);
        }

        itemQuery = itemQuery.Where(p => p.IdProject == projectID);

        return await itemQuery.ToListAsync();
    }

    public async Task<ItemDefinition?> Save(ItemDefinition itemDefinition)
    {
        using ApplicationDbContext context =
            await contextFactory.CreateDbContextAsync();
        // This is done, to establish tracking of objects, so we do not add multiple
        // data points at once
        ItemDefinition? item = context.ItemDefinitions.Include(i => i.Assets)
                                   .FirstOrDefault(i => i.Id == itemDefinition.Id);
        if (item == null || !await accessControlService.CheckUserAccessRightsWrite(
                                item.IdProject))
            return null;

        context.Entry(item).CurrentValues.SetValues(itemDefinition);

        if (itemDefinition.TechnicalSketch is not null)
        {
            Image trackedImage =
                context.Attach(itemDefinition.TechnicalSketch).Entity;
            item.TechnicalSketch = trackedImage;
        }
        if (itemDefinition.PreliminaryArchitecture is not null)
        {
            Image trackedImage =
                context.Attach(itemDefinition.PreliminaryArchitecture).Entity;
            item.PreliminaryArchitecture = trackedImage;
        }
        if (itemDefinition.ItemBoundary is not null)
        {
            Image trackedImage = context.Attach(itemDefinition.ItemBoundary).Entity;
            item.ItemBoundary = trackedImage;
        }
        if (itemDefinition.OperationalEnvironmentImage is not null)
        {
            Image trackedImage =
                context.Attach(itemDefinition.OperationalEnvironmentImage).Entity;
            item.OperationalEnvironmentImage = trackedImage;
        }

        item.Assets.Clear();

        foreach (Asset obj in item.Assets)
        {
            Asset tracked = context.Attach(obj).Entity;
            item.Assets.Add(tracked);
        }

        await context.SaveChangesAsync();

        return item;
    }

    public async Task Delete(ItemDefinition itemDefinition)
    {
        using ApplicationDbContext context =
            await contextFactory.CreateDbContextAsync();
        ItemDefinition? item =
            context.ItemDefinitions.FirstOrDefault(i => i.Id == itemDefinition.Id);
        if (item == null || !await accessControlService.CheckUserAccessRightsWrite(
                                item.IdProject))
            return;

        // Gets all the Assets which are only connected to this
        await foreach (Asset lonelyAsset in item.Assets
                           .Where(a => a.IdItemDefinition == itemDefinition.Id)
                           .ToAsyncEnumerable())
        {
            // Needs to be reactivated, when Assets are there
            // await assetService.Delete(lonelyAsset);
        }

        context.ItemDefinitions.Remove(item);
        await context.SaveChangesAsync();
    }

    public GridItemsProvider<ItemDefinition> GetItemsProvider(
        long ProjectId,
        Func<IQueryable<ItemDefinition>, IQueryable<ItemDefinition>>? include =
            null,
        Func<IQueryable<ItemDefinition>, IQueryable<ItemDefinition>>? filter =
            null)
    {
        return async request =>
        {
            await using ApplicationDbContext context =
                await contextFactory.CreateDbContextAsync(request.CancellationToken);
            if (await accessControlService.CheckUserAccessRightsRead(ProjectId)
                    is false)
            {
                return GridItemsProviderResult.From(new List<ItemDefinition>(), 0);
            }
            IQueryable<ItemDefinition> itemDefinitions =
                context.ItemDefinitions.AsNoTracking();

            if (include != null)
            {
                itemDefinitions = include(itemDefinitions);
            }

            if (filter != null)
            {
                itemDefinitions = filter(itemDefinitions);
            }

            int total = await itemDefinitions.CountAsync();
            List<ItemDefinition> items = await request.ApplySorting(itemDefinitions.OrderBy(i => i.ItemNumber))
                                             .Skip(request.StartIndex)
                                             .Take(request.Count ?? 20)
                                             .ToListAsync(request.CancellationToken);
            return GridItemsProviderResult.From(items, total);
        };
    }

    public async Task<(List<ItemDefinition>, int TotalItems)> GetItems(
        long ProjectId,
        Func<IQueryable<ItemDefinition>, IQueryable<ItemDefinition>>? include =
            null,
        Func<IQueryable<ItemDefinition>, IQueryable<ItemDefinition>>? filter =
            null)
    {
        using ApplicationDbContext context =
            await contextFactory.CreateDbContextAsync();

        if (await accessControlService.CheckUserAccessRightsRead(ProjectId)
                is false)
        {
            return ([], 0);
        }

        IQueryable<ItemDefinition> items = context.ItemDefinitions;

        if (include is not null)
        {
            items = include(items);
        }

        items = items.Where(i => i.IdProject == ProjectId);

        if (filter is not null)
        {
            items = filter(items);
        }

        return (await items.ToListAsync(), await items.CountAsync());
    }

    public async Task<int> CountAllItems(
        long ProjectId,
        Func<IQueryable<ItemDefinition>, IQueryable<ItemDefinition>>? filter =
            null)
    {
        using ApplicationDbContext context =
            await contextFactory.CreateDbContextAsync();
        if (await accessControlService.CheckUserAccessRightsRead(ProjectId)
                is false)
        {
            return 0;
        }

        IQueryable<ItemDefinition> items = context.ItemDefinitions;

        items = items.Where(a => a.IdProject == ProjectId);

        if (filter is not null)
        {
            items = filter(items);
        }

        return await items.CountAsync();
    }
}
