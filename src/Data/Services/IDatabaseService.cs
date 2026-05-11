using Microsoft.Build.Framework;
using Microsoft.FluentUI.AspNetCore.Components;

public interface IDataService<T> where T : class
{
    /// <summary>
    /// Saves and Entity of type T, automatically adds it to the Database, if not needed.
    /// </summary>
    /// <param name="entityToSave">The Entity to save</param>
    /// <returns>Return the saved/edited entity</returns>
    public Task<T?> Save(T entityToSave);

    /// <summary>
    /// Gets a single element by its ID. Also checks for access rights.
    /// </summary>
    /// <param name="Id">The ID of the element.</param>
    /// <param name="include">A lambda function, used to include additional tables, related to the element. </param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<T?> GetItemByIdAsync(long id, Func<IQueryable<T>, IQueryable<T>>? include = null, CancellationToken cancellationToken = default);

    public GridItemsProvider<T> GetItemsProvider(long ProjectId, Func<IQueryable<T>, IQueryable<T>>? include = null, Func<IQueryable<T>, IQueryable<T>>? filter = null);

    /// <summary>
    /// This function is needed to load the items for the breadcrumbs etc. This is needed to prevent the entire data from loading. This will be inefficient otherwise.
    /// </summary>
    /// <param name="ProjectId">The Id of the project</param>
    /// <param name="request">The r</pequest, if null, all items will be loaded. </param>
    /// <param name="filter"></param>
    /// <returns>A List, containing Pairs of the ID and Title of the Object. </returns>
    public Task<List<KeyValuePair<long, string>>> GetItems(long ProjectId, GridItemsProviderRequest<KeyValuePair<long, string>>? request = null, Func<IQueryable<T>, IQueryable<T>>? filter = null);

    /// <summary>
    /// Deletes the item and all preceding Data, which will become inreachable through the deletion of this object.
    /// </summary>
    /// <param name="itemToDelete">The Item to delete. </param>
    /// <returns></returns>
    public Task Delete(T itemToDelete);


}