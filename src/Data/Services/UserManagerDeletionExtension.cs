using Microsoft.AspNetCore.Identity;
using tara_tool.Data.Tables;
using Microsoft.EntityFrameworkCore;

namespace tara_tool.Data.Services;

public static class UserManagerDeletionExtension
{
    extension(UserManager<ApplicationUser> self)
    {
        /// <summary>
        /// Deletes a <paramref name="user"/> and then their projects, or, if <paramref name="transferRecipient"/> is set, transfers them.
        /// </summary>
        public async Task<IdentityResult> DeleteUserAndProjectsAsync(ApplicationUser user, ApplicationUser? transferRecipient = null)
        {
            ProjectService projectService = self.ServiceProvider.GetRequiredService<ProjectService>();
            AccessControlService accessControlService = self.ServiceProvider.GetRequiredService<AccessControlService>();

            List<Project> projects = await projectService.GetOwnedProjectsAsync(user);

            IdentityResult result = await self.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return result;
            }

            await accessControlService.DeleteOrphansAsync();

            if (transferRecipient is not null)
            {
                result = await TransferNewlyOrphanedProjects(self, projects, targetUser: transferRecipient);
                if (!result.Succeeded)
                {
                    return result;
                }
            }
            else
            {
                try
                {
                    projects.ForEach(async p => await projectService.Delete(p));
                }
                catch (Exception ex)
                {
                    return IdentityResult.Failed([new IdentityError() { Description = ex.Message }]);
                }
            }

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

        /// <summary>
        /// Transfers a list of projects to the <paramref name="targetUser"/>.
        /// </summery>
        private async Task<IdentityResult> TransferNewlyOrphanedProjects
        (
            List<Project> projects, ApplicationUser targetUser
        )
        {
            ProjectService projectService = self.ServiceProvider.GetRequiredService<ProjectService>();

            try
            {
                List<Exception> oex = [];
                projects.ForEach(async p =>
                {
                    try
                    {
                        if (!await projectService.TransferOrphanedProjectAsync(p.Id, targetUser))
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
