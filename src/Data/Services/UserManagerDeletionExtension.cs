using Microsoft.AspNetCore.Identity;
using tara_tool.Data.Tables;
using Microsoft.EntityFrameworkCore;

namespace tara_tool.Data.Services;

public static class UserManagerDeletionExtension
{
    extension(UserManager<ApplicationUser> self)
    {
        public async Task<IdentityResult> DeleteUserAndProjectsAsync(ApplicationUser user)
        {
            ProjectService projectService = self.ServiceProvider.GetRequiredService<ProjectService>();

            List<Project> projects = await projectService.GetOwnedProjectsAsync(user);

            try
            {
                projects.ForEach(async p => await projectService.Delete(p));
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed([new IdentityError() { Description = ex.Message }]);
            }

            return await self.DeleteAsync(user);
        }

        public async Task<IdentityResult> TransferAllProjects(ApplicationUser sourceUser, ApplicationUser targetUser)
        {
            ProjectService projectService = self.ServiceProvider.GetRequiredService<ProjectService>();

            List<Project> projects = await projectService.GetOwnedProjectsAsync(sourceUser);

            try
            {
                projects.ForEach(async p =>
                {
                    if (!await projectService.TransferOwnershipAsync(p.Id, targetUser))
                        throw new Exception($"Failed to transfer project {p.ProjectName}");
                });
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed([new IdentityError() { Description = ex.Message }]);
            }

            return IdentityResult.Success;
        }
    }
}
