using tara_tool.Data.Tabels;

namespace tara_tool.Data.Services;


public class AccessControlService(ApplicationDbContext context, SessionService sessionService)
{
    public async Task<bool> CheckUserAccessRightsRead(long ProjectId)
    {
        ApplicationUser? user = await sessionService.GetApplicationUserAsync();
        return user != null && context.AccessControls.Any(a => a.Project.Id == ProjectId && a.ApplicationUser.Id == user.Id && a.ReadAccess == true);
    }

    public async Task<bool> CheckUserAccessRightsWrite(long ProjectId)
    {
        ApplicationUser? user = await sessionService.GetApplicationUserAsync();
        return user != null && context.AccessControls.Any(a => a.Project.Id == ProjectId && a.ApplicationUser.Id == user.Id && a.WriteAccess == true);
    }

    public async Task<bool> CheckUserAccessRightsManage(long ProjectId)
    {
        ApplicationUser? user = await sessionService.GetApplicationUserAsync();
        return user != null && context.AccessControls.Any(a => a.Project.Id == ProjectId && a.ApplicationUser.Id == user.Id && a.Manage == true);
    }

    public async Task<bool> CheckUserAccessRightsOwner(long ProjectId)
    {
        ApplicationUser? user = await sessionService.GetApplicationUserAsync();
        return user != null && context.AccessControls.Any(a => a.Project.Id == ProjectId && a.ApplicationUser.Id == user.Id && a.Owner == true);
    }
}