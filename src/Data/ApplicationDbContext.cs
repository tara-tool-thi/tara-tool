using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace tara_tool.Data;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
  // DbSets
  public DbSet<ApplicationUser> ApplicationUsers { get; set; }
  public DbSet<AccessControl> AccessControls { get; set; }
  public DbSet<Project> Projects { get; set; }
  public DbSet<ItemDefinition> ItemDefinitions { get; set; }
  public DbSet<Image> Images { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);
    // Build the Model, by defining its relations
    builder.Entity<AccessControl>()
        .HasOne(e => e.Project)
        .WithMany(e => e.Access);
    builder.Entity<AccessControl>()
        .HasOne(e => e.Member)
        .WithMany(e => e.Projects);
    builder.Entity<Project>()
        .HasMany(e => e.ItemDefinitions)
        .WithOne(e => e.Project)
        .HasForeignKey(e => e.IdProject);
  }
}
