using Microsoft.AspNetCore.Identity;

namespace tara_tool.Data.Tables;

// Add profile data for application users by adding properties to the
// ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string? Organization { get; set; }
    public string? NameOfUser { get; set; }
    public virtual ICollection<AccessControl> Projects { get; set; } = [];

}
