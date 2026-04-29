using Microsoft.EntityFrameworkCore;
using tara_tool.Data.Tables;
namespace tara_tool.Data.Services;


public class AccessControlService(IDbContextFactory<ApplicationDbContext> contextFactory, SessionService sessionService)
{
    public async Task<bool> CheckUserAccessRightsRead(long ProjectId)
    {
        ApplicationUser? user = await sessionService.GetApplicationUserAsync();
        if (user is null)
        {
            return false;
        }

        await using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        return await context.AccessControls.AnyAsync(a => a.Project.Id == ProjectId && a.ApplicationUser.Id == user.Id && a.ReadAccess == true);
    }

    public async Task<bool> CheckUserAccessRightsWrite(long ProjectId)
    {
        ApplicationUser? user = await sessionService.GetApplicationUserAsync();

        await using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        return user != null && context.AccessControls.Any(a =>
            a.Project.Id == ProjectId &&
            a.ApplicationUser.Id == user.Id &&
            a.WriteAccess == true &&
            !a.Project.IsArchived);
    }

    public async Task<bool> CheckUserAccessRightsManage(long ProjectId)
    {
        ApplicationUser? user = await sessionService.GetApplicationUserAsync();
        if (user is null)
        {
            return false;
        }

        await using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        return await context.AccessControls.AnyAsync(a => a.Project.Id == ProjectId && a.ApplicationUser.Id == user.Id && a.Manage == true);
    }

    public async Task<bool> CheckUserAccessRightsOwner(long ProjectId)
    {
        ApplicationUser? user = await sessionService.GetApplicationUserAsync();
        if (user is null)
        {
            return false;
        }

        await using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        return await context.AccessControls.AnyAsync(a => a.Project.Id == ProjectId && a.ApplicationUser.Id == user.Id && a.Owner == true);
    }
}
