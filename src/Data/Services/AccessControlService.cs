using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tara_tool.Data.Tables;
namespace tara_tool.Data.Services;


public class AccessControlService(IDbContextFactory<ApplicationDbContext> contextFactory, SessionService sessionService, UserManager<ApplicationUser> userManager)
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
        if (user is null)
        {
            return false;
        }

        await using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
        return await context.AccessControls.AnyAsync(a =>
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

    ///<summary>
    /// Deletes all AccessControl items for <paramref name="user"/>.
    ///</summary>
    public async Task<bool> DeleteUserAccessesAsync(ApplicationUser user)
    {
        try
        {
            await using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();

            context.AccessControls.AttachRange(user.Projects);
            context.AccessControls.RemoveRange(user.Projects);
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteOrphansAsync()
    {
        try
        {
            await using ApplicationDbContext context = await contextFactory.CreateDbContextAsync();
            context.AccessControls.RemoveRange(
                context.AccessControls.Include(a => a.ApplicationUser).Where(a => userManager.FindByIdAsync(a.ApplicationUser.Id).Result == null)
            );
            await context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
