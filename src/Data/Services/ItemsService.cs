using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore;
using tara_tool.Components.Pages;
using tara_tool.Data.Services;

namespace tara_tool.Data;

public class ItemsService(ApplicationDbContext context, AccessControlService accessControlService)
{
    public async Task<ItemDefinition?> CreateItemAsync(long projectID, string name)
    {
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
        newItem.Project.DateLastChanged = DateTime.Now;
        await context.SaveChangesAsync();

        return newItem;
    }

    public async Task<ItemDefinition?> RetrieveItemDefinitionInfoAsync(long id)
    {
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
}