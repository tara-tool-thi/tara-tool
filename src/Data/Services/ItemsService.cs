using Microsoft.EntityFrameworkCore;

namespace tara_tool.Data;

public class ItemsService(ApplicationDbContext context)
{    
    public async Task<ItemDefinition> CreateItemAsync(long projectID, string name)
    {
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

    public async Task<ItemDefinition?> RetrieveItemDefinitionInfoAsync(long itemID)
    {
        ItemDefinition? itemDefinition = await context.ItemDefinitions
            .Include(item => item.TechnicalSketch)
            .FirstOrDefaultAsync(item => item.Id == itemID);

        return itemDefinition;
    }

    public async Task SaveItemAsync(ItemDefinition item)
    {
        if (context.Entry(item).State == EntityState.Detached)
        {
            context.ItemDefinitions.Update(item);
        }

        if (item.TechnicalSketch != null &&
            context.Entry(item.TechnicalSketch).State == EntityState.Detached)
        {
            context.Images.Add(item.TechnicalSketch);
        }

        await context.SaveChangesAsync();
    }
}