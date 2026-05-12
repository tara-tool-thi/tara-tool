using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class DamageScenarioUpdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DamageScenarios_Assets_AssetId",
                table: "DamageScenarios");

            migrationBuilder.DropTable(
                name: "DamageScenarioThreatScenario");

            migrationBuilder.DropColumn(
                name: "availabilityAffected",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "confidentialityAffected",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "financial",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "integrityAffected",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "operational",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "privacy",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "safety",
                table: "DamageScenarios");

            migrationBuilder.AddColumn<long>(
                name: "DamageScenariosId",
                table: "ThreatScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "AssetId",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThreatScenarios_DamageScenariosId",
                table: "ThreatScenarios",
                column: "DamageScenariosId");

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
                name: "FK_DamageScenarios_Assets_AssetId",
                table: "DamageScenarios");

            migrationBuilder.DropForeignKey(
                name: "FK_ThreatScenarios_DamageScenarios_DamageScenariosId",
                table: "ThreatScenarios");

            migrationBuilder.DropIndex(
                name: "IX_ThreatScenarios_DamageScenariosId",
                table: "ThreatScenarios");

            migrationBuilder.DropColumn(
                name: "DamageScenariosId",
                table: "ThreatScenarios");

            migrationBuilder.AlterColumn<long>(
                name: "AssetId",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<bool>(
                name: "availabilityAffected",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "confidentialityAffected",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "financial",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "integrityAffected",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "operational",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "privacy",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "safety",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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
                name: "IX_DamageScenarioThreatScenario_ThreatScenariosId",
                table: "DamageScenarioThreatScenario",
                column: "ThreatScenariosId");

            migrationBuilder.AddForeignKey(
                name: "FK_DamageScenarios_Assets_AssetId",
                table: "DamageScenarios",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");
        }
    }
}
