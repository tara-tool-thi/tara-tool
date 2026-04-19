using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using tara_tool.Data;

namespace tara_tool.Data.Services;

public class ItemsService(IDbContextFactory<ApplicationDbContext> contextFactory, AccessControlService accessControlService)
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

    public async Task<ItemDefinition?> RetrieveItemDefinitionInfoAsync(long id)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        ItemDefinition? itemDefinition = await context.ItemDefinitions.Include(i => i.Project).AsQueryable().FirstOrDefaultAsync(i => i.Id == id);
        if (itemDefinition is null)
        {
            return null;
        }
        if (await accessControlService.CheckUserAccessRightsRead(itemDefinition.Project.Id) is false)
        {
            return null;
        }

        return itemDefinition;
    }

    public async Task Save(ItemDefinition itemDefinition)
    {
        using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        //This is done, to establish tracking of objects, so we do not add multiple data points at once
        ItemDefinition? item = context.ItemDefinitions.Include(i => i.Assets).FirstOrDefault(i => i.Id == itemDefinition.Id);
        if (item == null)
        {
            List<Asset> assets = itemDefinition.Assets.ToList();
        }
    }
}