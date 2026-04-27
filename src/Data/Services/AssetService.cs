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
        throw new NotImplementedException();
    }

    public async Task<bool> CheckIfAssetNumberIsAvailable(long IdItemDefinition, long AssetNumber)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

        ItemDefinition? itemDefinition = await context.ItemDefinitions.FirstOrDefaultAsync(i => i.Id == IdItemDefinition);
        return !itemDefinition?.Assets.Any(a => a.AssetNumber == AssetNumber) ?? false;
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
    public GridItemsProvider<Asset> GetItemsProvider(Func<IQueryable<Asset>, IQueryable<Asset>>? include = null, Func<IQueryable<Asset>, IQueryable<Asset>>? filter = null)
    {
        throw new NotImplementedException();
    }

    public async Task<Asset?> Save(Asset entityToSave)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        Asset? asset = context.Assets.FirstOrDefault(a => a.Id == entityToSave.Id);
        if (asset is null || await accessControlService.CheckUserAccessRightsWrite(asset.ItemDefinitions.First().IdProject) is false)
        {
            return null;
        }
        context.Entry(asset).CurrentValues.SetValues(entityToSave);
        asset.AssetGroup.Clear();
        foreach (var t in entityToSave.AssetGroup)
        {
            //Done to establish tracking of the Entitys
            Tag? tag = await tagService.GetItemByIdAsync(t.Id);
            if (tag is null)
            {
                continue;
            }

            asset.AssetGroup.Add(tag);
        }

        await context.SaveChangesAsync();
        return asset;
    }

    public async Task<Asset?> GetItemByIdAsync(long Id, Func<IQueryable<Asset>, IQueryable<Asset>>? include = null, CancellationToken cancellationToken = default)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        Asset? asset = context.Assets.FirstOrDefault(a => a.Id == Id);
        if (asset is null || await accessControlService.CheckUserAccessRightsRead(asset.ItemDefinitions.First().IdProject) is false)
        {
            return null;
        }

        return asset;
    }
}