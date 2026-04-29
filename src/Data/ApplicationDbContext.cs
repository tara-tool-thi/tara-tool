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
    public DbSet<Project> Projects { get; set; }
    public DbSet<ItemDefinition> ItemDefinitions { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<DamageScenario> DamageScenarios { get; set; }
    public DbSet<ImpactRating> ImpactRatings { get; set; }
    public DbSet<TreatmentDecision> TreatmentDecisions { get; set; }
    public DbSet<ThreatScenario> ThreatScenarios { get; set; }
    public DbSet<AttackPath> AttackPaths { get; set; }

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
        builder.Entity<Asset>()
            .HasMany(e => e.DamageScenarios)
            .WithMany(e => e.Assets);
        builder.Entity<DamageScenario>()
            .HasMany(e => e.ThreatScenarios)
            .WithMany(e => e.DamageScenarios);
        builder.Entity<ThreatScenario>()
            .HasMany(e => e.AttackPaths)
            .WithMany(e => e.ThreatScenarios);
        builder.Entity<ImpactRating>()
            .HasOne(e => e.DamageScenario)
            .WithOne(e => e.ImpactRating)
            .HasForeignKey<ImpactRating>(e => e.DamageScenarioId)
            .IsRequired(true);
        builder.Entity<TreatmentDecision>()
            .HasOne(e => e.ImpactRating)
            .WithOne(e => e.TreatmentDecision)
            .HasForeignKey<TreatmentDecision>(e => e.ImpactRatingId)
            .IsRequired(true);
        builder.Entity<Tag>()
            .HasOne(e => e.Project)
            .WithMany(e => e.Tags)
            .HasForeignKey(e => e.IdProject)
            .IsRequired(true);
    }
}
