using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.FluentUI.AspNetCore.Components;
using tara_tool.Data.Tables;

namespace tara_tool.Data.Services;

public class ItemDefinitionService(IDbContextFactory<ApplicationDbContext> contextFactory, AccessControlService accessControlService) : IDataService<ItemDefinition>
{
    public async Task<ItemDefinition?> CreateItemAsync(long projectID, string name)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        if (await accessControlService.CheckUserAccessRightsWrite(projectID) == false)
        {
            return null;
        }

        ItemDefinition newItem = new ItemDefinition
        {
            ItemName = name,
            Project = await context.Projects.FindAsync(projectID)
            ?? throw new Exception("Invalid Project ID for Item Creation")
        };

        context.ItemDefinitions.Add(newItem);
        newItem.Project.DateLastChanged = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return newItem;
    }

    public async Task<ItemDefinition?> GetItemByIdAsync(long id, Func<IQueryable<ItemDefinition>, IQueryable<ItemDefinition>>? include = null, CancellationToken cancellationToken = default)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        IQueryable<ItemDefinition> itemDefinitions = context.ItemDefinitions.AsQueryable();
        if (include is not null)
        {
            itemDefinitions = include(itemDefinitions);
        }
        ItemDefinition? itemDefinition = await itemDefinitions.FirstOrDefaultAsync(i => i.Id == id);
        if (itemDefinition is null)
        {
            return null;
        }
        if (await accessControlService.CheckUserAccessRightsRead(itemDefinition.IdProject) is false)
        {
            return null;
        }

        return itemDefinition;
    }

    public async Task<ItemDefinition?> Save(ItemDefinition itemDefinition)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        //This is done, to establish tracking of objects, so we do not add multiple data points at once
        ItemDefinition? item = context.ItemDefinitions.Include(i => i.Assets).FirstOrDefault(i => i.Id == itemDefinition.Id);
        if (item == null)
        {
            return null;
        }
        context.Entry(item).CurrentValues.SetValues(itemDefinition);

        if (itemDefinition.TechnicalSketch is not null)
        {
            Image trackedImage = context.Attach(itemDefinition.TechnicalSketch).Entity;
            item.TechnicalSketch = trackedImage;
        }
        if (itemDefinition.PreliminaryArchitecture is not null)
        {
            Image trackedImage = context.Attach(itemDefinition.PreliminaryArchitecture).Entity;
            item.TechnicalSketch = trackedImage;
        }
        if (itemDefinition.ItemBoundary is not null)
        {
            Image trackedImage = context.Attach(itemDefinition.ItemBoundary).Entity;
            item.TechnicalSketch = trackedImage;
        }
        if (itemDefinition.OperationalEnvironmentImage is not null)
        {
            Image trackedImage = context.Attach(itemDefinition.OperationalEnvironmentImage).Entity;
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
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        ItemDefinition? item = context.ItemDefinitions.FirstOrDefault(i => i.Id == itemDefinition.Id);
        if (item is null)
        {
            return;
        }

        //Gets all the Assets which are only connected to this


        context.ItemDefinitions.Remove(item);
        await context.SaveChangesAsync();
    }

    public GridItemsProvider<ItemDefinition> GetItemsProvider(long ProjectId, Func<IQueryable<ItemDefinition>, IQueryable<ItemDefinition>>? include = null, Func<IQueryable<ItemDefinition>, IQueryable<ItemDefinition>>? filter = null)
    {
        return async request =>
        {
            await using ApplicationDbContext context = await contextFactory.CreateDbContextAsync(request.CancellationToken);
            if (await accessControlService.CheckUserAccessRightsRead(ProjectId) is false)
            {
                return GridItemsProviderResult.From(new List<ItemDefinition>(), 0);
            }
            IQueryable<ItemDefinition> itemDefinitions = context.ItemDefinitions;

            if (include != null)
            {
                itemDefinitions = include(itemDefinitions);
            }

            if (filter != null)
            {
                itemDefinitions = filter(itemDefinitions);
            }

            int total = await itemDefinitions.CountAsync();
            List<ItemDefinition> items = await itemDefinitions.Skip(request.StartIndex).Take(request.Count ?? 20).ToListAsync(request.CancellationToken);
            return GridItemsProviderResult.From(items, total);
        };
    }

}