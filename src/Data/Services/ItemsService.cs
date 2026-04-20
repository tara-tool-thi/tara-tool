using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.FluentUI.AspNetCore.Components;
using tara_tool.Data.Tabels;

namespace tara_tool.Data.Services;

public class ItemDefinitionService(IDbContextFactory<ApplicationDbContext> contextFactory, AccessControlService accessControlService, AssetService assetService) : IDataService<ItemDefinition>
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
        context.Entry(item).CurrentValues.SetValues(item);

        IQueryable<Asset> existing = item.Assets.AsQueryable();
        List<Asset> incoming = itemDefinition.Assets.ToList();

        foreach (Asset asset in incoming)
        {
            if (!existing.Any(a => a.Id == asset.Id))
            {
                context.Attach(asset);
                item.Assets.Add(asset);
            }
        }

        foreach (Asset asset in existing)
        {
            if (!incoming.Any(a => a.Id == asset.Id))
            {

                item.Assets.Remove(asset);
            }
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
        await foreach (Asset lonelyAsset in item.Assets.Where(a => a.ItemDefinitions.Count() == 1 && a.ItemDefinitions.Any(i => i.Id == itemDefinition.Id)).ToAsyncEnumerable())
        {
            await assetService.Delete(lonelyAsset);
        }

        context.ItemDefinitions.Remove(item);
        await context.SaveChangesAsync();
    }

    public GridItemsProvider<ItemDefinition> GetItemsProvider(Func<IQueryable<ItemDefinition>, IQueryable<ItemDefinition>>? include = null, Func<IQueryable<ItemDefinition>, IQueryable<ItemDefinition>>? filter = null)
    {
        return async request =>
        {
            await using ApplicationDbContext context = await contextFactory.CreateDbContextAsync(request.CancellationToken);
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