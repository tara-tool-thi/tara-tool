namespace tara_tool.Data;

public class ItemsService(ApplicationDbContext context)
{    
    public async Task CreateItemAsync(long projectID, string name)
        {
            ItemDefinition Item = new ItemDefinition
            {
                ItemName = name,
                Project = await context.Projects.FindAsync(projectID) ?? throw new Exception("Invalid Project ID for Item Creation")
            };

            context.ItemDefinitions.Add(Item);
            await context.SaveChangesAsync();

            return;
        }
}