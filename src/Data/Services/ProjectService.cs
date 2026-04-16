using Microsoft.EntityFrameworkCore;
using tara_tool.Data;
using tara_tool.Data.Services;
using tara_tool.Data.Tabels;

public class ProjectService(
    IDbContextFactory<ApplicationDbContext> contextFactory, AccessControlService accessControlService, SessionService sessionService)
{

    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory =
        contextFactory;

    public async Task<Project?> GetProjectAsync(long id, Func<DbSet<Project>, IQueryable<Project>>? extend)
    {
        bool access = await accessControlService.CheckUserAccessRightsRead(id);
        if (access == false)
        {
            return null;
        }
        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();

        DbSet<Project> set = context.Projects;
        IQueryable<Project> query = set.AsQueryable();
        if (extend != null)
        {
            query = extend.Invoke(context.Projects);
        }
        return await set.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Project>>
    GetProjectsAsync(Func<DbSet<Project>, IQueryable<Project>>? extend = null)
    {
        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();
        DbSet<Project> set = context.Projects;
        IQueryable<Project> projectQuery = set.AsQueryable();
        if (extend != null)
        {
            projectQuery = extend.Invoke(set);
        }
        ApplicationUser? user = await sessionService.GetApplicationUserAsync();
        if (user is null)
        {
            return new List<Project>();
        }
        //If Owner == null then the project will be shown - Ardwetha
        projectQuery = projectQuery.Where(
          p => p.Access.Select(a => a.ApplicationUser.Id).Contains(user.Id));

        return await projectQuery.ToListAsync();
    }

    public async Task<Project?> CreateNewProjectAsync(string name)
    {
        ApplicationUser? user = await sessionService.GetApplicationUserAsync();
        if (user is null)
        {
            return null;
        }
        using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
        Project newProject = new Project
        {
            ProjectName = name
        };


        await context.Projects.AddAsync(newProject);
        await context.SaveChangesAsync();

        context.Projects.Attach(newProject);
        context.ApplicationUsers.Attach(user);

        AccessControl accessControl = new AccessControl
        {
            ReadAccess = true,
            WriteAccess = true,
            Manage = true,
            Owner = true,
            Project = newProject,
            ApplicationUser = user,
        };

        await context.AccessControls.AddAsync(accessControl);
        await context.SaveChangesAsync();

        return newProject;

    }

    //This feature is here to prevent 
    public async Task AddUserToProjectAsync(long ProjectId, bool Read = true, bool Write = false, bool Manage = false, bool Owner = false)
    {
        using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
        Project? project = await GetProjectAsync(ProjectId, (set) => set.Include(a => a.Access));
        ApplicationUser? user = await sessionService.GetApplicationUserAsync();
        if (user is null || project is null || await accessControlService.CheckUserAccessRightsManage(ProjectId) is false)
        {
            return;
        }

        //More than one Owner should never exist
        if (project.Access.Count(p => p.Owner == true) > 0 && Owner == true)
        {
            return;
        }

        context.Projects.Attach(project);
        context.Users.Attach(user);

        AccessControl accessControl = new AccessControl
        {
            ReadAccess = Read,
            WriteAccess = Write,
            Manage = Manage,
            Owner = Owner,
            Project = project,
            ApplicationUser = user,
        };

        await context.AccessControls.AddAsync(accessControl);
        await context.SaveChangesAsync();
    }
}
