using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using tara_tool.Data;
using tara_tool.Data.Services;
using tara_tool.Data.Tables;

public class ProjectService(
    IDbContextFactory<ApplicationDbContext> contextFactory,
    AccessControlService accessControlService, SessionService sessionService,
    ItemDefinitionService itemDefinitionService)
    : IDataService<Project>
{

    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory =
        contextFactory;

    public async Task<string?> ExportProjectAsync(long projectId)
    {
        if (!await accessControlService.CheckUserAccessRightsManage(projectId))
        {
            return null;
        }

        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();

        // Load the full graph
        Project? project = await context.Projects.Include(p => p.ItemDefinitions)
                               .ThenInclude(id => id.TechnicalSketch)
                               .Include(p => p.ItemDefinitions)
                               .ThenInclude(id => id.PreliminaryArchitecture)
                               .Include(p => p.ItemDefinitions)
                               .ThenInclude(id => id.ItemBoundary)
                               .Include(p => p.ItemDefinitions)
                               .ThenInclude(id => id.OperationalEnvironmentImage)
                               .Include(p => p.ItemDefinitions)
                               .ThenInclude(id => id.Assets)
                               .ThenInclude(a => a.Tag)
                               .Include(p => p.ItemDefinitions)
                               .ThenInclude(id => id.Assets)
                               .ThenInclude(a => a.DamageScenarios)
                               .ThenInclude(ds => ds.ThreatScenarios)
                               .ThenInclude(ts => ts.AttackPaths)
                               .ThenInclude(ap => ap.Steps)
                               .Include(p => p.Tags)
                               .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            return null;

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            ReferenceHandler =
                                                      ReferenceHandler.Preserve,
            WriteIndented = true
        };

        return JsonSerializer.Serialize(project, options);
    }

    public async Task<bool> ImportProjectAsync(string json)
    {
        try
        {
            JsonSerializerOptions options =
                new() { ReferenceHandler = ReferenceHandler.Preserve };

            Project? project = JsonSerializer.Deserialize<Project>(json, options);
            if (project == null)
                return false;

            ApplicationUser? user = await sessionService.GetApplicationUserAsync();
            if (user == null)
                return false;

            using ApplicationDbContext context =
                await _contextFactory.CreateDbContextAsync();

            // Reset IDs and metadata to ensure everything is treated as new
            project.Id = 0;
            project.DateCreated = DateTime.UtcNow;
            project.DateLastChanged = DateTime.UtcNow;
            project.IsArchived = false;



            // Build canonical set from project.Tags (same instances as asset.Tag after deserialization)
            HashSet<Tag> canonicalTags = new(ReferenceEqualityComparer.Instance);
            foreach (Tag tag in project.Tags)
                canonicalTags.Add(tag);



            foreach (ItemDefinition itemDef in project.ItemDefinitions)
            {
                itemDef.Id = 0;
                itemDef.IdProject = 0;
                itemDef.Project = project;

                if (itemDef.TechnicalSketch != null)
                    itemDef.TechnicalSketch.Id = 0;
                if (itemDef.PreliminaryArchitecture != null)
                    itemDef.PreliminaryArchitecture.Id = 0;
                if (itemDef.ItemBoundary != null)
                    itemDef.ItemBoundary.Id = 0;
                if (itemDef.OperationalEnvironmentImage != null)
                    itemDef.OperationalEnvironmentImage.Id = 0;

                foreach (Asset asset in itemDef.Assets)
                {
                    asset.Id = 0;
                    asset.IdItemDefinition = 0;
                    asset.ItemDefinition = itemDef;

                    if (asset.Tag != null)
                    {
                        if (asset.Tag != null)
                            asset.IdTag = 0;
                    }

                    foreach (DamageScenario ds in asset.DamageScenarios)
                    {
                        ds.Id = 0;
                        ds.Asset = asset;
                        foreach (ThreatScenario ts in ds.ThreatScenarios)
                        {
                            ts.Id = 0;
                            ts.DamageScenarios = ds;
                            foreach (AttackPath ap in ts.AttackPaths)
                            {
                                ap.Id = 0;
                                ap.ThreatScenarios = ts;
                                foreach (AttackStep step in ap.Steps)
                                {
                                    step.Id = 0;
                                    step.AttackPath = ap;
                                }
                            }
                        }
                    }
                }
            }

            foreach (Tag tag in project.Tags)
            {
                tag.Id = 0;
                tag.Project = project;
            }

            // Attach user to context so it's not treated as a new entity
            context.ApplicationUsers.Attach(user);

            await context.Projects.AddAsync(project);

            // Assign current user as owner
            AccessControl accessControl = new()
            {
                ReadAccess = true,
                WriteAccess = true,
                Manage = true,
                Owner = true,
                Project = project,
                ApplicationUser = user,
            };

            await context.AccessControls.AddAsync(accessControl);
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<Project?>
    GetItemByIdAsync(long id,
                     Func<IQueryable<Project>, IQueryable<Project>>? extend,
                     CancellationToken cancellationToken = default)
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
        // If Owner == null then the project will be shown - Ardwetha
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
        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();
        Project newProject = new Project { ProjectName = name };

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

    // This feature is here to prevent
    public async Task AddUserToProjectAsync(long ProjectId, bool Read = true,
                                            bool Write = false,
                                            bool Manage = false,
                                            bool Owner = false)
    {
        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();
        Project? project =
            await GetItemByIdAsync(ProjectId, (set) => set.Include(a => a.Access));
        ApplicationUser? user = await sessionService.GetApplicationUserAsync();
        if (user is null || project is null ||
            await accessControlService.CheckUserAccessRightsManage(ProjectId)
                is false)
        {
            return;
        }

        // More than one Owner should never exist
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
        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();

        Project? project = await context.Projects.FirstOrDefaultAsync(
            p => p.Id == entityToSave.Id);

        if (project == null ||
            !await accessControlService.CheckUserAccessRightsWrite(project.Id))
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
            // Item was added
            if (!existing.Any(i => i.Id == item.Id))
            {
                // Establish tracking
                context.Attach(item);
                project.ItemDefinitions.Add(item);
            }
        }

        foreach (ItemDefinition item in existing)
        {
            // Item was removed
            if (!incoming.Any(i => i.Id == item.Id))
            {
                // No tracking needed due to it beeing tracked automatically
                project.ItemDefinitions.Remove(item);
            }
        }

        await context.SaveChangesAsync();
        return project;
    }

    public GridItemsProvider<Project> GetItemsProvider(
        Func<IQueryable<Project>, IQueryable<Project>>? include = null,
        Func<IQueryable<Project>, IQueryable<Project>>? filter = null)
    {
        return GetItemsProvider(0, include, filter);
    }

    public GridItemsProvider<Project> GetItemsProvider(
        long ProjectId,
        Func<IQueryable<Project>, IQueryable<Project>>? include = null,
        Func<IQueryable<Project>, IQueryable<Project>>? filter = null)
    {
        return async request =>
        {
            await using ApplicationDbContext context =
                await _contextFactory.CreateDbContextAsync(request.CancellationToken);
            ApplicationUser? user = await sessionService.GetApplicationUserAsync();
            if (user is null)
            {
                return GridItemsProviderResult.From(new List<Project>(), 0);
            }

            IQueryable<Project> projects = context.Projects.AsNoTracking().Where(
                p => p.Access.Any(a => a.ApplicationUser.Id == user.Id));

            if (include != null)
            {
                projects = include(projects);
            }

            if (filter != null)
            {
                projects = filter(projects);
            }

            int total = await projects.CountAsync();
            List<Project> items = await request.ApplySorting(projects)
                                      .Skip(request.StartIndex)
                                      .Take(request.Count ?? 20)
                                      .ToListAsync(request.CancellationToken);
            return GridItemsProviderResult.From(items, total);
        };
    }

    public async Task<List<AccessControl>>
    GetProjectMembersAsync(long projectId)
    {
        bool hasManage =
            await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage)
        {
            return [];
        }

        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();
        return await context.AccessControls.Where(a => a.Project.Id == projectId)
            .Include(a => a.ApplicationUser)
            .ToListAsync();
    }

    public async Task<bool> UpdateMemberRoleAsync(long projectId,
                                                  long accessControlId, bool read,
                                                  bool write, bool manage)
    {
        bool hasManage =
            await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage)
        {
            return false;
        }

        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();
        AccessControl? ac = await context.AccessControls.FirstOrDefaultAsync(
            a => a.Id == accessControlId);

        if (ac == null || ac.Owner)
        {
            return false;
        }

        ac.ReadAccess = read;
        ac.WriteAccess = write;
        ac.Manage = manage;

        await context.Projects.Where(p => p.Id == projectId).ExecuteUpdateAsync(setters => setters.SetProperty(p => p.DateLastChanged, DateTime.UtcNow));

        await context.SaveChangesAsync();
        return true;
    }

    public async Task RemoveMemberFromProjectAsync(long projectId,
                                                   long accessControlId)
    {
        bool hasManage =
            await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage)
        {
            return;
        }

        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();
        AccessControl? ac = await context.AccessControls.FirstOrDefaultAsync(
            a => a.Id == accessControlId && a.Project.Id == projectId);

        if (ac == null || ac.Owner)
        {
            return;
        }

        context.AccessControls.Remove(ac);

        await context.Projects.Where(p => p.Id == projectId).ExecuteUpdateAsync(setters => setters.SetProperty(p => p.DateLastChanged, DateTime.UtcNow));

        await context.SaveChangesAsync();
    }

    public async Task UnarchiveProjectAsync(long projectId)
    {
        bool hasManage =
            await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage)
        {
            return;
        }

        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();
        Project? project =
            await context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
        {
            return;
        }

        project.IsArchived = false;
        project.DateLastChanged = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    public async Task<bool> TransferOwnershipAsync(long projectId,
                                                   long newOwnerAccessControlId)
    {
        bool isOwner =
            await accessControlService.CheckUserAccessRightsOwner(projectId);
        if (!isOwner)
        {
            return false;
        }

        ApplicationUser? currentUser =
            await sessionService.GetApplicationUserAsync();
        if (currentUser == null)
        {
            return false;
        }

        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();

        AccessControl? currentOwnerAc =
            await context.AccessControls.FirstOrDefaultAsync(
                a => a.Project.Id == projectId &&
                     a.ApplicationUser.Id == currentUser.Id && a.Owner);

        AccessControl? newOwnerAc =
            await context.AccessControls.FirstOrDefaultAsync(
                a => a.Id == newOwnerAccessControlId && a.Project.Id == projectId &&
                     !a.Owner);

        if (currentOwnerAc == null || newOwnerAc == null)
        {
            return false;
        }

        currentOwnerAc.Owner = false;
        newOwnerAc.Owner = true;
        newOwnerAc.Manage = true;
        newOwnerAc.WriteAccess = true;
        newOwnerAc.ReadAccess = true;

        await context.Projects.Where(p => p.Id == projectId).ExecuteUpdateAsync(setters => setters.SetProperty(p => p.DateLastChanged, DateTime.UtcNow));

        await context.SaveChangesAsync();
        return true;
    }

    public async Task UpdateProjectNameAsync(long projectId, string newName)
    {
        bool hasManage =
            await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage)
        {
            return;
        }

        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();
        Project? project =
            await context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
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
        bool hasManage =
            await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage)
        {
            return;
        }

        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();
        Project? project =
            await context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
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
        using ApplicationDbContext applicationDbContext =
            await _contextFactory.CreateDbContextAsync();

        Project? project = await applicationDbContext.Projects.FirstOrDefaultAsync(
            i => i.Id == itemToDelete.Id);

        if (project is null)
        {
            return;
        }

        await foreach (AccessControl accessControl in project.Access
                           .ToAsyncEnumerable())
        {
            applicationDbContext.AccessControls.Remove(accessControl);
        }

        await applicationDbContext.SaveChangesAsync();

        await foreach (ItemDefinition itemDefinition in project.ItemDefinitions
                           .ToAsyncEnumerable())
        {
            await itemDefinitionService.Delete(itemDefinition);
        }

        applicationDbContext.Projects.Remove(project);
        await applicationDbContext.SaveChangesAsync();
    }

    public async Task<string?> InviteMemberByEmailAsync(long projectId,
                                                        string email, bool read,
                                                        bool write, bool manage)
    {
        bool hasManage =
            await accessControlService.CheckUserAccessRightsManage(projectId);
        if (!hasManage)
            return "No permission.";

        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();

        ApplicationUser? targetUser =
            await context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email ==
                                                                    email);
        if (targetUser == null)
            return "User not found.";

        bool alreadyMember = await context.AccessControls.AnyAsync(
            a =>
                a.Project.Id == projectId && a.ApplicationUser.Id == targetUser.Id);
        if (alreadyMember)
            return "User is already a member.";

        Project? project =
            await context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
            return "Project not found.";

        AccessControl accessControl = new AccessControl
        {
            ReadAccess = read,
            WriteAccess = write,
            Manage = manage,
            Owner = false,
            Project = project,
            ApplicationUser = targetUser,
        };

        project.DateLastChanged = DateTime.UtcNow;

        await context.AccessControls.AddAsync(accessControl);
        await context.SaveChangesAsync();
        return null;
    }

    // Overload, because the interface forces projectId which we do not have, when
    // checking all projects
    public async Task<(List<Project>, int TotalItems)>
    GetItems(Func<IQueryable<Project>, IQueryable<Project>>? include = null,
             Func<IQueryable<Project>, IQueryable<Project>>? filter = null)
    {
        return await GetItems(0, include, filter);
    }

    public async Task<(List<Project>, int TotalItems)>
    GetItems(long ProjectId,
             Func<IQueryable<Project>, IQueryable<Project>>? include = null,
             Func<IQueryable<Project>, IQueryable<Project>>? filter = null)
    {
        using ApplicationDbContext context =
            await _contextFactory.CreateDbContextAsync();

        // Manual check for this one, because what Project ID should I choose? Using
        // everysingle one will break tracking -> killing the performance
        string? IdUser = (await sessionService.GetApplicationUserAsync())?.Id;
        if (IdUser == null)
            return ([], 0);

        // We need to check the access rights of the user right here
        IQueryable<Project> projects = context.Projects.Where(
            p => p.Access.Any(a => a.ApplicationUser.Id == IdUser));

        if (include is not null)
        {
            projects = include(projects);
        }
        if (filter is not null)
        {
            projects = filter(projects);
        }

        return (await projects.ToListAsync(), await projects.CountAsync());
    }
}
