using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class RebaseDbStructureFromDSscenario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetDamageScenario");

            migrationBuilder.DropTable(
                name: "AttackPathThreatScenario");

            migrationBuilder.DropTable(
                name: "DamageScenarioThreatScenario");

            migrationBuilder.DropIndex(
                name: "IX_ImpactRatings_DamageScenarioId",
                table: "ImpactRatings");

            migrationBuilder.RenameColumn(
                name: "IntegrityImpact",
                table: "DamageScenarios",
                newName: "Safety");

            migrationBuilder.RenameColumn(
                name: "ConfidentialityImpact",
                table: "DamageScenarios",
                newName: "Privacy");

            migrationBuilder.RenameColumn(
                name: "AvailabilityImpcat",
                table: "DamageScenarios",
                newName: "Operational");

            migrationBuilder.AddColumn<long>(
                name: "DamageScenariosId",
                table: "ThreatScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AssetId",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "AvailabilityAffected",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConfidentialityAffected",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Financial",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IntegrityAffected",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ThreatScenariosId",
                table: "AttackPaths",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ThreatScenarios_DamageScenariosId",
                table: "ThreatScenarios",
                column: "DamageScenariosId");

            migrationBuilder.CreateIndex(
                name: "IX_ImpactRatings_DamageScenarioId",
                table: "ImpactRatings",
                column: "DamageScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageScenarios_AssetId",
                table: "DamageScenarios",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AttackPaths_ThreatScenariosId",
                table: "AttackPaths",
                column: "ThreatScenariosId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttackPaths_ThreatScenarios_ThreatScenariosId",
                table: "AttackPaths",
                column: "ThreatScenariosId",
                principalTable: "ThreatScenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DamageScenarios_Assets_AssetId",
                table: "DamageScenarios",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThreatScenarios_DamageScenarios_DamageScenariosId",
                table: "ThreatScenarios",
                column: "DamageScenariosId",
                principalTable: "DamageScenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttackPaths_ThreatScenarios_ThreatScenariosId",
                table: "AttackPaths");

            migrationBuilder.DropForeignKey(
                name: "FK_DamageScenarios_Assets_AssetId",
                table: "DamageScenarios");

            migrationBuilder.DropForeignKey(
                name: "FK_ThreatScenarios_DamageScenarios_DamageScenariosId",
                table: "ThreatScenarios");

            migrationBuilder.DropIndex(
                name: "IX_ThreatScenarios_DamageScenariosId",
                table: "ThreatScenarios");

            migrationBuilder.DropIndex(
                name: "IX_ImpactRatings_DamageScenarioId",
                table: "ImpactRatings");

            migrationBuilder.DropIndex(
                name: "IX_DamageScenarios_AssetId",
                table: "DamageScenarios");

            migrationBuilder.DropIndex(
                name: "IX_AttackPaths_ThreatScenariosId",
                table: "AttackPaths");

            migrationBuilder.DropColumn(
                name: "DamageScenariosId",
                table: "ThreatScenarios");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "AvailabilityAffected",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "ConfidentialityAffected",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "Financial",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "IntegrityAffected",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "ThreatScenariosId",
                table: "AttackPaths");

            migrationBuilder.RenameColumn(
                name: "Safety",
                table: "DamageScenarios",
                newName: "IntegrityImpact");

            migrationBuilder.RenameColumn(
                name: "Privacy",
                table: "DamageScenarios",
                newName: "ConfidentialityImpact");

            migrationBuilder.RenameColumn(
                name: "Operational",
                table: "DamageScenarios",
                newName: "AvailabilityImpcat");

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

            migrationBuilder.CreateIndex(
                name: "IX_ImpactRatings_DamageScenarioId",
                table: "ImpactRatings",
                column: "DamageScenarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetDamageScenario_DamageScenariosId",
                table: "AssetDamageScenario",
                column: "DamageScenariosId");

            migrationBuilder.CreateIndex(
                name: "IX_AttackPathThreatScenario_ThreatScenariosId",
                table: "AttackPathThreatScenario",
                column: "ThreatScenariosId");

            migrationBuilder.CreateIndex(
                name: "IX_DamageScenarioThreatScenario_ThreatScenariosId",
                table: "DamageScenarioThreatScenario",
                column: "ThreatScenariosId");
        }
    }
}
