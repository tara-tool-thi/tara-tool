using Microsoft.FluentUI.AspNetCore.Components;
using tara_tool.Data.Tabels;

public class AssetService : IDataService<Asset>
{
    //Needs to be implemented by the Person creating the assets
    public async Task Delete(Asset itemToDelete)
    {
        throw new NotImplementedException();
    }

    public GridItemsProvider<Asset> GetItemsProvider(Func<IQueryable<Asset>, IQueryable<Asset>>? include = null, Func<IQueryable<Asset>, IQueryable<Asset>>? filter = null)
    {
        throw new NotImplementedException();
    }

    public async Task<Asset?> Save(Asset entityToSave)
    {
        throw new NotImplementedException();
    }

    public Task<Asset?> GetItemByIdAsync(long Id, Func<IQueryable<Asset>, IQueryable<Asset>>? include, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}