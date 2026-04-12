namespace tara_tool.Data.Services;


public class AccessControlService(ApplicationDbContext context)
{
    public async Task<bool> CheckUserAccessRightsRead(long ProjectId, ApplicationUser user)
    {
        AccessControl? accessControl = context.AccessControls
                                        .SingleOrDefault(p => p.Project.Id == ProjectId &&
                                        p.Member.Id == user!.Id &&
                                        p.ReadAccess == true);

        if(accessControl == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public async Task<bool> CheckUserAccessRightsWrite(long ProjectId, ApplicationUser user)
    {
        AccessControl? accessControl = context.AccessControls
                                        .SingleOrDefault(p => p.Project.Id == ProjectId &&
                                        p.Member.Id == user!.Id &&
                                        p.WriteAccess == true);

        if(accessControl == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public async Task<bool> CheckUserAccessRightsManage(long ProjectId, ApplicationUser user)
    {
        AccessControl? accessControl = context.AccessControls
                                        .SingleOrDefault(p => p.Project.Id == ProjectId &&
                                        p.Member.Id == user!.Id &&
                                        p.Manage == true);

        if(accessControl == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public async Task<bool> CheckUserAccessRightsOwner(long ProjectId, ApplicationUser user)
    {
        AccessControl? accessControl = context.AccessControls
                                        .SingleOrDefault(p => p.Project.Id == ProjectId &&
                                        p.Member.Id == user!.Id &&
                                        p.Owner == true);

        if(accessControl == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}