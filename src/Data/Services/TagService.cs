using Microsoft.EntityFrameworkCore;
using tara_tool.Data;
using tara_tool.Data.Services;
using tara_tool.Data.Tables;

public class TagService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    AccessControlService accessControlService)
{
    public async Task<Tag?> CreateTag(long IdCurrentProject, string Name)
    {
        if (await accessControlService.CheckUserAccessRightsWrite(IdCurrentProject)
                is false)
        {
            return null;
        }

        using ApplicationDbContext context =
            await dbContextFactory.CreateDbContextAsync();
        Project? project = await context.Projects.FirstOrDefaultAsync(
            p => p.Id == IdCurrentProject);

        if (project == null)
            return null;

        Tag tag = new() { Project = project, Name = Name };

        Tag? existing = await context.Tags.FirstOrDefaultAsync(
            t =>
                t.Project!.Id == IdCurrentProject && t.Name.ToLower().Equals(Name));

        if (existing is null)
        {
            await context.AddAsync(tag);
            await context.SaveChangesAsync();
        }

        return existing is null ? tag : existing;
    }

    public async Task<Tag?> GetItemByIdAsync(long id)
    {
        using ApplicationDbContext context =
            await dbContextFactory.CreateDbContextAsync();
        Tag? tag = await context.Tags.Include(a => a.Project)
                       .AsNoTracking()
                       .FirstOrDefaultAsync(t => t.Id == id);
        if (tag is null)
        {
            return null;
        }

        if (await accessControlService.CheckUserAccessRightsRead(tag.Project!.Id)
                is false)
        {
            return null;
        }

        return tag;
    }

    public async Task<List<Tag>> GetAllTagsInProject(long idProject)
    {
        if (await accessControlService.CheckUserAccessRightsRead(idProject)
                is false)
        {
            return [];
        }

        using ApplicationDbContext context =
            await dbContextFactory.CreateDbContextAsync();

        Project? project = await context.Projects.AsNoTracking()
                               .Include(p => p.Tags)
                               .FirstOrDefaultAsync(p => p.Id == idProject);

        if (project is null)
        {
            return [];
        }

        return [.. project.Tags];
    }

    public async Task Delete(Tag tag)
    {
        using ApplicationDbContext context =
            await dbContextFactory.CreateDbContextAsync();


        Tag? foundTag = await context.Tags.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == tag.Id);

        if (foundTag is null)
            return;

        if (await accessControlService.CheckUserAccessRightsWrite(foundTag.Project!.Id) is false) return;
        context.Tags.Remove(foundTag);
        await context.SaveChangesAsync();
    }
}
