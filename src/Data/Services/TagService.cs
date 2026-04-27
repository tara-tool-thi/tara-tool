using Microsoft.EntityFrameworkCore;
using tara_tool.Data;
using tara_tool.Data.Services;
using tara_tool.Data.Tables;

public class TagService(IDbContextFactory<ApplicationDbContext> dbContextFactory, AccessControlService accessControlService)
{
    public async Task<Tag?> CreateTag(long IdCurrentProject, string Name)
    {
        if (await accessControlService.CheckUserAccessRightsWrite(IdCurrentProject) is false)
        {
            return null;
        }

        using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync();
        Tag tag = new Tag
        {
            IdProject = IdCurrentProject,
            Name = Name
        };

        Tag? existing = await context.Tags.FirstOrDefaultAsync(t => t.IdProject == IdCurrentProject && t.Name.ToLower().Equals(Name));

        if (existing is null)
        {
            await context.AddAsync(tag);
            await context.SaveChangesAsync();
        }

        return existing is null ? tag : existing;
    }

    public async Task<Tag?> GetItemByIdAsync(long id)
    {
        using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync();
        Tag? tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == id);
        if (tag is null)
        {
            return null;
        }

        if (await accessControlService.CheckUserAccessRightsRead(tag.IdProject) is false)
        {
            return null;
        }

        return tag;

    }
}