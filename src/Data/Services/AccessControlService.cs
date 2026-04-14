using SQLitePCL;

namespace tara_tool.Data.Services;


public class AccessControlService(ApplicationDbContext context)
{
    public bool CheckUserAccessRightsRead(long ProjectId, ApplicationUser user)
    {
        return context.AccessControls.Any(a => a.Project.Id == ProjectId && a.Member.Id == user!.Id && a.ReadAccess == true);
    }

    public bool CheckUserAccessRightsWrite(long ProjectId, ApplicationUser user)
    {
        return context.AccessControls.Any(a => a.Project.Id == ProjectId && a.Member.Id == user!.Id && a.WriteAccess == true);
    }

    public bool CheckUserAccessRightsManage(long ProjectId, ApplicationUser user)
    {
        return context.AccessControls.Any(a => a.Project.Id == ProjectId && a.Member.Id == user!.Id && a.Manage == true);
    }

    public bool CheckUserAccessRightsOwner(long ProjectId, ApplicationUser user)
    {
        return context.AccessControls.Any(a => a.Project.Id == ProjectId && a.Member.Id == user!.Id && a.Owner == true);
    }
}