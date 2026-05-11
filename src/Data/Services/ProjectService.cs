using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.FluentUI.AspNetCore.Components;
using tara_tool.Data;
using tara_tool.Data.Services;
using tara_tool.Data.Tables;

public class ProjectService(
    IDbContextFactory<ApplicationDbContext> contextFactory, AccessControlService accessControlService, SessionService sessionService, ItemDefinitionService itemDefinitionService) : IDataService<Project>
{

    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory =
        contextFactory;

    public async Task<Project?> GetItemByIdAsync(long id, Func<IQueryable<Project>, IQueryable<Project>>? extend, CancellationToken cancellationToken = default)
    {
        bool access = await accessControlService.CheckUserAccessRightsRead(id);
        if (access == false)
        {
            return null;
        }
        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();

        DbSet<Project> set = context.Projects;
        IQueryable<Project> query = set.AsNoTracking().AsQueryable();
        if (extend != null)
        {
            query = extend(query);
        }
        return await set.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Project>>
    GetProjectsAsync(Func<DbSet<Project>, IQueryable<Project>>? extend = null)
    {
        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();
        DbSet<Project> set = context.Projects;
        IQueryable<Project> projectQuery = set.AsNoTracking().AsQueryable();
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
        Project? project = await GetItemByIdAsync(ProjectId, (set) => set.Include(a => a.Access));
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

    public async Task<Project?> Save(Project entityToSave)
    {
        using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();

        Project? project = await context.Projects.FirstOrDefaultAsync(p => p.Id == entityToSave.Id);

        if (project == null || !await accessControlService.CheckUserAccessRightsWrite(project.Id))
        {
            return null;
        }
        context.Entry(project).CurrentValues.SetValues(entityToSave);
        await context.SaveChangesAsync();

        List<ItemDefinition> incoming = entityToSave.ItemDefinitions.ToList();
        IQueryable<ItemDefinition> existing = project.ItemDefinitions.AsQueryable();

        foreach (ItemDefinition item in incoming)
        {
            if (item.Id == 0)
            {
                await context.ItemDefinitions.AddAsync(item);
            }
            //Item was added
            if (!existing.Any(i => i.Id == item.Id))
            {
                //Establish tracking
                context.Attach(item);
                project.ItemDefinitions.Add(item);
            }
        }

        foreach (ItemDefinition item in existing)
        {
            //Item was removed
            if (!incoming.Any(i => i.Id == item.Id))
            {
                //No tracking needed due to it beeing tracked automatically
                project.ItemDefinitions.Remove(item);
            }
        }

        await context.SaveChangesAsync();
        return project;
    }

    //Not yet relevant
    public GridItemsProvider<Project> GetItemsProvider(long ProjectId, Func<IQueryable<Project>, IQueryable<Project>>? include = null, Func<IQueryable<Project>, IQueryable<Project>>? filter = null)
    {
        throw new NotImplementedException();
    }

    public async Task<List<AccessControl>> GetProjectMembersAsync(long projectId)
    {
        bool hasManage = await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage)
        {
            return [];
        }

        using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
        return await context.AccessControls
            .Where(a => a.Project.Id == projectId)
            .Include(a => a.ApplicationUser)
            .ToListAsync();
    }

    public async Task<bool> UpdateMemberRoleAsync(long projectId, long accessControlId, bool read, bool write, bool manage)
    {
        bool hasManage = await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage)
        {
            return false;
        }

        using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
        AccessControl? ac = await context.AccessControls
            .FirstOrDefaultAsync(a => a.Id == accessControlId);

        if (ac == null || ac.Owner)
        {
            return false;
        }

        ac.ReadAccess = read;
        ac.WriteAccess = write;
        ac.Manage = manage;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task RemoveMemberFromProjectAsync(long projectId, long accessControlId)
    {
        bool hasManage = await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage)
        {
            return;
        }

        using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
        AccessControl? ac = await context.AccessControls
            .FirstOrDefaultAsync(a => a.Id == accessControlId && a.Project.Id == projectId);

        if (ac == null || ac.Owner)
        {
            return;
        }

        context.AccessControls.Remove(ac);
        await context.SaveChangesAsync();
    }

    public async Task UnarchiveProjectAsync(long projectId)
    {
        bool hasManage = await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage)
        {
            return;
        }

        using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
        Project? project = await context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
        {
            return;
        }

        project.IsArchived = false;
        project.DateLastChanged = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    public async Task<bool> TransferOwnershipAsync(long projectId, long newOwnerAccessControlId)
    {
        bool isOwner = await accessControlService.CheckUserAccessRightsOwner(projectId);
        if (!isOwner)
        {
            return false;
        }

        ApplicationUser? currentUser = await sessionService.GetApplicationUserAsync();
        if (currentUser == null)
        {
            return false;
        }

        using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();

        AccessControl? currentOwnerAc = await context.AccessControls
            .FirstOrDefaultAsync(a => a.Project.Id == projectId && a.ApplicationUser.Id == currentUser.Id && a.Owner);

        AccessControl? newOwnerAc = await context.AccessControls
            .FirstOrDefaultAsync(a => a.Id == newOwnerAccessControlId && a.Project.Id == projectId && !a.Owner);

        if (currentOwnerAc == null || newOwnerAc == null)
        {
            return false;
        }

        currentOwnerAc.Owner = false;
        newOwnerAc.Owner = true;
        newOwnerAc.Manage = true;
        newOwnerAc.WriteAccess = true;
        newOwnerAc.ReadAccess = true;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task UpdateProjectNameAsync(long projectId, string newName)
    {
        bool hasManage = await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage)
        {
            return;
        }

        using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
        Project? project = await context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
        {
            return;
        }

        project.ProjectName = newName;
        project.DateLastChanged = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    public async Task ArchiveProjectAsync(long projectId)
    {
        bool hasManage = await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage)
        {
            return;
        }

        using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();
        Project? project = await context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
        {
            return;
        }

        project.IsArchived = true;
        project.DateLastChanged = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    public async Task Delete(Project itemToDelete)
    {
        using ApplicationDbContext applicationDbContext = await _contextFactory.CreateDbContextAsync();

        Project? project = await applicationDbContext.Projects.FirstOrDefaultAsync(i => i.Id == itemToDelete.Id);

        if (project is null)
        {
            return;
        }

        await foreach (AccessControl accessControl in project.Access.ToAsyncEnumerable())
        {
            applicationDbContext.AccessControls.Remove(accessControl);
        }

        await applicationDbContext.SaveChangesAsync();

        await foreach (ItemDefinition itemDefinition in project.ItemDefinitions.ToAsyncEnumerable())
        {
            await itemDefinitionService.Delete(itemDefinition);
        }

        applicationDbContext.Projects.Remove(project);
        await applicationDbContext.SaveChangesAsync();
    }

    public async Task<string?> InviteMemberByEmailAsync(long projectId, string email, bool read, bool write, bool manage)
    {
        bool hasManage = await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage) return "No permission.";

        using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();

        ApplicationUser? targetUser = await context.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Email == email);
        if (targetUser == null) return "User not found.";

        bool alreadyMember = await context.AccessControls
            .AnyAsync(a => a.Project.Id == projectId && a.ApplicationUser.Id == targetUser.Id);
        if (alreadyMember) return "User is already a member.";

        Project? project = await context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null) return "Project not found.";

        AccessControl accessControl = new AccessControl
        {
            ReadAccess = read,
            WriteAccess = write,
            Manage = manage,
            Owner = false,
            Project = project,
            ApplicationUser = targetUser,
        };

        await context.AccessControls.AddAsync(accessControl);
        await context.SaveChangesAsync();
        return null;
    }

    public async Task<List<KeyValuePair<long, string>>> GetItems(long ProjectId, GridItemsProviderRequest<KeyValuePair<long, string>>? request = null, Func<IQueryable<Project>, IQueryable<Project>>? filter = null)
    {
        using ApplicationDbContext context = await _contextFactory.CreateDbContextAsync();

        if (await accessControlService.CheckUserAccessRightsRead(ProjectId) is false) return [];

        IQueryable<Project> projects = context.Projects;

        if (filter is not null)
        {
            projects = filter(projects);
        }

        if (request is not null)
        {
            projects = projects.Skip(request.Value.StartIndex).Take(request.Value.Count ?? 20);
        }

        return await projects.Select(a => new KeyValuePair<long, string>(a.Id, a.ProjectName)).ToListAsync();
    }
}
