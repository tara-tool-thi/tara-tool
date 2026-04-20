using Microsoft.AspNetCore.Identity;

namespace tara_tool.Data.Tabels;

// Add profile data for application users by adding properties to the
// ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ICollection<AccessControl> Projects { get; set; } = [];
}
