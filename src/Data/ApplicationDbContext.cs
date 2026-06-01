using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tara_tool.Data.Tables;

namespace tara_tool.Data;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    // DbSets
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<AccessControl> AccessControls { get; set; }
    public DbSet<PendingRegistration> PendingRegistrations { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ItemDefinition> ItemDefinitions { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<DamageScenario> DamageScenarios { get; set; }
    public DbSet<TreatmentDecision> TreatmentDecisions { get; set; }
    public DbSet<ThreatScenario> ThreatScenarios { get; set; }
    public DbSet<AttackPath> AttackPaths { get; set; }
    public DbSet<AttackStep> AttackSteps { get; set; }

    static public void GetDbConfig(DbContextOptionsBuilder databaseBuilder,
                                   WebApplicationBuilder builder)
    {
        // We will look up the config file here later and connect to either sqlite
        // or postgress
        string? connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found.");

        databaseBuilder.UseSqlite(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Build the Model, by defining its relations
        builder.Entity<ApplicationUser>().Ignore(e => e.EmailConfirmed);
        builder.Entity<AccessControl>()
            .HasOne(e => e.Project)
            .WithMany(e => e.Access)
            .IsRequired(true);
        builder.Entity<AccessControl>()
            .HasOne(e => e.ApplicationUser)
            .WithMany(e => e.Projects)
            .IsRequired(true);
        builder.Entity<ItemDefinition>()
            .HasOne(e => e.Project)
            .WithMany(e => e.ItemDefinitions)
            .HasForeignKey(e => e.IdProject)
            .IsRequired(true);
        builder.Entity<Asset>()
            .HasOne(e => e.ItemDefinition)
            .WithMany(e => e.Assets)
            .HasForeignKey(e => e.IdItemDefinition)
            .IsRequired(true);
        builder.Entity<Asset>()
            .HasOne(e => e.Tag)
            .WithMany()
            .HasForeignKey(e => e.IdTag)
            .IsRequired(false);
        builder.Entity<DamageScenario>().HasKey(e => e.Id);
        builder.Entity<DamageScenario>()
            .HasOne(e => e.Asset)
            .WithMany(e => e.DamageScenarios)
            .IsRequired();
        builder.Entity<ThreatScenario>()
            .HasOne(e => e.DamageScenarios)
            .WithMany(e => e.ThreatScenarios)
            .IsRequired();
        builder.Entity<AttackPath>()
            .HasOne(e => e.ThreatScenarios)
            .WithMany(e => e.AttackPaths)
            .IsRequired();
        builder.Entity<AttackPath>()
            .HasMany(ap => ap.Steps)
            .WithOne()
            .HasForeignKey(step => step.AttackPathId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Tag>()
            .HasOne(e => e.Project)
            .WithMany(e => e.Tags)
            .HasForeignKey(e => e.IdProject)
            .IsRequired(true);
    }
}
