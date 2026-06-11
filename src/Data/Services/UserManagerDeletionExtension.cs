using Microsoft.AspNetCore.Identity;
using tara_tool.Data.Tables;
using Microsoft.EntityFrameworkCore;

namespace tara_tool.Data.Services;

public static class UserManagerDeletionExtension
{
    extension(UserManager<ApplicationUser> self)
    {
        /// <summary>
        /// Deletes a user’s projects, accesses and then the user itself.
        /// </summary>
        public async Task<IdentityResult> DeleteUserAndProjectsAsync(ApplicationUser user)
        {
            ProjectService projectService = self.ServiceProvider.GetRequiredService<ProjectService>();
            AccessControlService accessControlService = self.ServiceProvider.GetRequiredService<AccessControlService>();

            List<Project> projects = await projectService.GetOwnedProjectsAsync(user);

            try
            {
                projects.ForEach(async p => await projectService.Delete(p));
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed([new IdentityError() { Description = ex.Message }]);
            }

            if (!await accessControlService.DeleteUserAccessesAsync(user))
            {
                return IdentityResult.Failed([new IdentityError() { Description = "Failed to delete all accesses of user." }]);
            }

            // Prevents “Optimistic concurrency failure: Object has been modified”
            IdentityResult result = await self.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return result;
            }

            // await accessControlService.DeleteOrphansAsync();

            return result;
        }

        /// <summary>
        /// Transfers all projects owned by <paramref name="sourceUser"/> to <paramref name="targetUser"/>.
        /// </summary>
        public async Task<IdentityResult> TransferAllProjects
        (
            ApplicationUser sourceUser, ApplicationUser targetUser, bool doNotVerifyOwnership = false
        )
        {
            ProjectService projectService = self.ServiceProvider.GetRequiredService<ProjectService>();

            List<Project> projects = await projectService.GetOwnedProjectsAsync(sourceUser);

            try
            {
                List<Exception> oex = [];
                projects.ForEach(async p =>
                {
                    try
                    {
                        if (!await projectService.TransferOwnershipAsync(p.Id, targetUser, doNotVerifyOwnership))
                            throw new Exception($"Failed to transfer project {p.ProjectName}");
                    }
                    catch (Exception ex)
                    {
                        oex.Add(ex);
                    }
                });
                if (oex is not [])
                {
                    throw new Exception(string.Join('\n', oex.Select(e => e.Message)));
                }
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed([new IdentityError() { Description = ex.Message }]);
            }

            return IdentityResult.Success;
        }
    }
}
