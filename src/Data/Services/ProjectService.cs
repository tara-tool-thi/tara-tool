namespace tara_tool.Data.Services;


public class ProjectService(ApplicationDbContext context)
{
    public async Task<Project> CreateProjectAsync(string name)
    {
        Project newProject = new Project
        {
            ProjectName = name
        };

        context.Projects.Add(newProject);
        await context.SaveChangesAsync();

        return newProject;
    }

    public async Task<Project?> RetrieveProjectInfoAsync(long ID)
    {
        Project? project = await context.Projects.FindAsync(ID);

        return project;
    }

}
