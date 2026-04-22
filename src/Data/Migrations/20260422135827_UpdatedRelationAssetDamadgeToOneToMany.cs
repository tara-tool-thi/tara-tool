using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRelationAssetDamadgeToOneToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetDamageScenario");

            migrationBuilder.AddColumn<long>(
                name: "AssetId",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_DamageScenarios_AssetId",
                table: "DamageScenarios",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_DamageScenarios_Assets_AssetId",
                table: "DamageScenarios",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DamageScenarios_Assets_AssetId",
                table: "DamageScenarios");

            migrationBuilder.DropIndex(
                name: "IX_DamageScenarios_AssetId",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "DamageScenarios");

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

            migrationBuilder.CreateIndex(
                name: "IX_AssetDamageScenario_DamageScenariosId",
                table: "AssetDamageScenario",
                column: "DamageScenariosId");
        }
    }
}
