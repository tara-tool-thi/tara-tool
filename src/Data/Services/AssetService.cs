using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using tara_tool.Data;
using tara_tool.Data.Services;
using tara_tool.Data.Tables;

public class AssetService(IDbContextFactory<ApplicationDbContext> contextFactory, AccessControlService accessControlService, TagService tagService) : IDataService<Asset>
{
    //Needs to be implemented by the Person creating the assets
    public async Task Delete(Asset itemToDelete)
    {

        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        Asset? asset = await context.Assets.Include(a => a.ItemDefinition).FirstOrDefaultAsync(a => a.Id == itemToDelete.Id);
        if (asset is null || await accessControlService.CheckUserAccessRightsWrite(asset.ItemDefinition!.IdProject) is false)
        {
            return;
        }
        List<Tag> tags = await tagService.GetAllTagsInProject(asset.ItemDefinition.IdProject);

        context.Remove(asset);
        await context.SaveChangesAsync();

        foreach (Tag tag in tags.Where(t => !context.Assets.Any(a => a.Tag == t)))
        {
            await tagService.Delete(tag);
        }

        await context.Projects.Where(p => p.Id == asset.ItemDefinition.IdProject).ExecuteUpdateAsync(setters => setters.SetProperty(p => p.DateLastChanged, DateTime.UtcNow));

        await context.SaveChangesAsync();
    }

    public async Task<Asset?> CreateNewAsset(long IdItemDefinition)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        ItemDefinition? itemDefinition = await context.ItemDefinitions.Include(i => i.Assets).FirstOrDefaultAsync(i => i.Id == IdItemDefinition);
        if (itemDefinition is null || await accessControlService.CheckUserAccessRightsWrite(itemDefinition.IdProject) is false)
        {
            return null;
        }

        Project? project = await context.Projects.Include(a => a.ItemDefinitions).ThenInclude(i => i.Assets).FirstOrDefaultAsync(p => p.Id == itemDefinition.IdProject);
        if (project is null) return null;

        IEnumerable<Asset> assets = project.ItemDefinitions.SelectMany(a => a.Assets);
        long number = 0;
        if (assets.Count() > 0)
        {
            number = assets.Max(a => a.AssetNumber) + 1;
        }

        Asset newAsset = new Asset
        {
            AssetNumber = number,
            AssetName = "New Asset",
            IdItemDefinition = IdItemDefinition //Adding ForeignKey
        };

        project.DateLastChanged = DateTime.UtcNow;

        await context.AddAsync(newAsset);
        await context.SaveChangesAsync();

        return newAsset;
    }

    public async Task<long?> GetAvailableAssetNumber(long IdItemDefinition)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        ItemDefinition? itemDefinition = await context.ItemDefinitions.FirstOrDefaultAsync(i => i.Id == IdItemDefinition);
        if (itemDefinition is null)
        {
            return null;
        }

        return itemDefinition.Assets.Max(a => a.AssetNumber) + 1;
    }
    public GridItemsProvider<Asset> GetItemsProvider(long ProjectId, Func<IQueryable<Asset>, IQueryable<Asset>>? include = null, Func<IQueryable<Asset>, IQueryable<Asset>>? filter = null)
    {
        return async request =>
        {
            await using ApplicationDbContext context = await contextFactory.CreateDbContextAsync(request.CancellationToken);



            if (await accessControlService.CheckUserAccessRightsRead(ProjectId) is false) return GridItemsProviderResult.From(new List<Asset>(), 0);

            IQueryable<Asset> Asset = context.Assets.AsNoTracking().Where(a => a.ItemDefinition!.IdProject == ProjectId);

            if (include != null)
            {
                Asset = include(Asset);
            }

            if (filter != null)
            {
                Asset = filter(Asset);
            }

            int total = await Asset.CountAsync();
            List<Asset> items = await request.ApplySorting(Asset).Skip(request.StartIndex).Take(request.Count ?? 20).ToListAsync(request.CancellationToken);
            return GridItemsProviderResult.From(items, total);
        };
    }

    public async Task<Asset?> Save(Asset entityToSave)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        Asset? asset = await context.Assets.Include(e => e.ItemDefinition).ThenInclude(e => e != null ? e.Project : null).FirstOrDefaultAsync(a => a.Id == entityToSave.Id);
        if (asset is null || await accessControlService.CheckUserAccessRightsWrite(asset.ItemDefinition!.IdProject) is false)
        {
            return null;
        }
        context.Entry(asset).CurrentValues.SetValues(entityToSave);

        if (entityToSave.Tag is not null)
        {
            asset.Tag = entityToSave.Tag;
        }
        else
        {
            //Update Relation explicitly
            asset.Tag = null;
            asset.IdTag = null;
        }

        asset.ItemDefinition.Project.DateLastChanged = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return asset;
    }

    public async Task<Asset?> GetItemByIdAsync(long Id, Func<IQueryable<Asset>, IQueryable<Asset>>? include = null, CancellationToken cancellationToken = default)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        IQueryable<Asset> assets = context.Assets.AsQueryable();
        if (include is not null)
        {
            assets = include(assets);
        }
        Asset? asset = await assets.Include(a => a.ItemDefinition).FirstOrDefaultAsync(a => a.Id == Id);
        if (asset is null || await accessControlService.CheckUserAccessRightsRead(asset.ItemDefinition!.IdProject) is false)
        {
            return null;
        }

        return asset;
    }

    public async Task<(List<Asset>, int TotalItems)> GetItems(long ProjectId, Func<IQueryable<Asset>, IQueryable<Asset>>? include = null, Func<IQueryable<Asset>, IQueryable<Asset>>? filter = null)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        if (await accessControlService.CheckUserAccessRightsRead(ProjectId) is false)
        {
            return ([], 0);
        }

        IQueryable<Asset> assets = context.Assets;

        if (include is not null)
        {
            assets = include(assets);
        }

        assets = assets.Where(a => a.ItemDefinition!.IdProject == ProjectId);

        if (filter is not null)
        {
            assets = filter(assets);
        }

        return (await assets.ToListAsync(), await assets.CountAsync());
    }

    public async Task<int> CountAllItems(long ProjectId, Func<IQueryable<Asset>, IQueryable<Asset>>? filter = null)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        if (await accessControlService.CheckUserAccessRightsRead(ProjectId) is false)
        {
            return 0;
        }

        IQueryable<Asset> assets = context.Assets;

        assets = assets.Where(a => a.ItemDefinition!.IdProject == ProjectId);

        if (filter is not null)
        {
            assets = filter(assets);
        }

        return await assets.CountAsync();
    }
}
