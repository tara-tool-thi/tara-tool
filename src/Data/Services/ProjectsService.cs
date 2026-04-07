namespace tara_tool.Data;

public class ProjectsService(ApplicationDbContext context)
{
    public async Task<Project> CreateProjectAsync(string name)
    {
        Project project = new Project
        {
            ProjectName = name
        };

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        return project;
    }

    public async Task<Project?> RetrieveProjectInfoAsync(long ID)
    {
        Project? project = await context.Projects.FindAsync(ID);

        return project;
    }

}
