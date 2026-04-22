using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssetNumber = table.Column<long>(type: "INTEGER", nullable: false),
                    AssetName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttackPaths",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ElapsedTime = table.Column<int>(type: "INTEGER", nullable: false),
                    SpecialistExpertise = table.Column<int>(type: "INTEGER", nullable: false),
                    KnowledgeOfComponents = table.Column<int>(type: "INTEGER", nullable: false),
                    WindowOfOpportunity = table.Column<int>(type: "INTEGER", nullable: false),
                    Equipment = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<long>(type: "INTEGER", nullable: false),
                    AttackFeasibilityRating = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttackPaths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DamageScenarios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ConfidentialityImpact = table.Column<bool>(type: "INTEGER", nullable: false),
                    IntegrityImpact = table.Column<bool>(type: "INTEGER", nullable: false),
                    AvailabilityImpcat = table.Column<bool>(type: "INTEGER", nullable: false),
                    DamageScenarioNumber = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageScenarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThreatScenarios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    StrideCategorie = table.Column<int>(type: "INTEGER", nullable: false),
                    RiskValue = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreatScenarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetItemDefinition",
                columns: table => new
                {
                    AssetsId = table.Column<long>(type: "INTEGER", nullable: false),
                    ItemDefinitionsId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetItemDefinition", x => new { x.AssetsId, x.ItemDefinitionsId });
                    table.ForeignKey(
                        name: "FK_AssetItemDefinition_Assets_AssetsId",
                        column: x => x.AssetsId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetItemDefinition_ItemDefinitions_ItemDefinitionsId",
                        column: x => x.ItemDefinitionsId,
                        principalTable: "ItemDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    AssetId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetDamageScenario",
                columns: table => new
                {
                    AssetsId = table.Column<long>(type: "INTEGER", nullable: false),
                    DamageScenariosId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDamageScenario", x => new { x.AssetsId, x.DamageScenariosId });
                    table.ForeignKey(
                        name: "FK_AssetDamageScenario_Assets_AssetsId",
                        column: x => x.AssetsId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetDamageScenario_DamageScenarios_DamageScenariosId",
                        column: x => x.DamageScenariosId,
                        principalTable: "DamageScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImpactRatings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DamageScenarioId = table.Column<long>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: false),
                    RiskValue = table.Column<long>(type: "INTEGER", nullable: false),
                    ImpactCategory = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Requirements = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpactRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImpactRatings_DamageScenarios_DamageScenarioId",
                        column: x => x.DamageScenarioId,
                        principalTable: "DamageScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttackPathThreatScenario",
                columns: table => new
                {
                    AttackPathsId = table.Column<long>(type: "INTEGER", nullable: false),
                    ThreatScenariosId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttackPathThreatScenario", x => new { x.AttackPathsId, x.ThreatScenariosId });
                    table.ForeignKey(
                        name: "FK_AttackPathThreatScenario_AttackPaths_AttackPathsId",
                        column: x => x.AttackPathsId,
                        principalTable: "AttackPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttackPathThreatScenario_ThreatScenarios_ThreatScenariosId",
                        column: x => x.ThreatScenariosId,
                        principalTable: "ThreatScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DamageScenarioThreatScenario",
                columns: table => new
                {
                    DamageScenariosId = table.Column<long>(type: "INTEGER", nullable: false),
                    ThreatScenariosId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamageScenarioThreatScenario", x => new { x.DamageScenariosId, x.ThreatScenariosId });
                    table.ForeignKey(
                        name: "FK_DamageScenarioThreatScenario_DamageScenarios_DamageScenariosId",
                        column: x => x.DamageScenariosId,
                        principalTable: "DamageScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DamageScenarioThreatScenario_ThreatScenarios_ThreatScenariosId",
                        column: x => x.ThreatScenariosId,
                        principalTable: "ThreatScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentDecisions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RiskTreatmentOption = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Requirements = table.Column<string>(type: "TEXT", nullable: false),
                    ImpactRatingId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentDecisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentDecisions_ImpactRatings_ImpactRatingId",
                        column: x => x.ImpactRatingId,
                        principalTable: "ImpactRatings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetDamageScenario_DamageScenariosId",
                table: "AssetDamageScenario",
                column: "DamageScenariosId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetItemDefinition_ItemDefinitionsId",
                table: "AssetItemDefinition",
                column: "ItemDefinitionsId");

            migrationBuilder.CreateIndex(
                name: "IX_AttackPathThreatScenario_ThreatScenariosId",
                table: "AttackPathThreatScenario",
                column: "ThreatScenariosId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageScenarioThreatScenario_ThreatScenariosId",
                table: "DamageScenarioThreatScenario",
                column: "ThreatScenariosId");

            migrationBuilder.CreateIndex(
                name: "IX_ImpactRatings_DamageScenarioId",
                table: "ImpactRatings",
                column: "DamageScenarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_AssetId",
                table: "Tags",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentDecisions_ImpactRatingId",
                table: "TreatmentDecisions",
                column: "ImpactRatingId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetDamageScenario");

            migrationBuilder.DropTable(
                name: "AssetItemDefinition");

            migrationBuilder.DropTable(
                name: "AttackPathThreatScenario");

            migrationBuilder.DropTable(
                name: "DamageScenarioThreatScenario");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "TreatmentDecisions");

            migrationBuilder.DropTable(
                name: "AttackPaths");

            migrationBuilder.DropTable(
                name: "ThreatScenarios");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "ImpactRatings");

            migrationBuilder.DropTable(
                name: "DamageScenarios");
        }
    }
}
