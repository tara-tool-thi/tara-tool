using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class newDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQLite-safe cleanup for legacy artifacts that may or may not exist.
            migrationBuilder.Sql("DROP TABLE IF EXISTS \"AssetItemDefinition\";");
            migrationBuilder.Sql("DROP TABLE IF EXISTS \"ImpactRating\";");
            migrationBuilder.Sql("DROP TABLE IF EXISTS \"DamageScenario\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssetId",
                table: "Tags",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssetItemDefinition",
                columns: table => new
                {
                    AssetsId = table.Column<long>(type: "INTEGER", nullable: true),
                    ItemDefinitionsId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
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
                name: "DamageScenario",
                columns: table => new
                {
                    TempId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.UniqueConstraint("AK_DamageScenario_TempId", x => x.TempId);
                });

            migrationBuilder.CreateTable(
                name: "ImpactRating",
                columns: table => new
                {
                    DamageScenarioId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_ImpactRating_DamageScenario_DamageScenarioId",
                        column: x => x.DamageScenarioId,
                        principalTable: "DamageScenario",
                        principalColumn: "TempId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Assets_AssetId",
                table: "Tags",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");
        }
    }
}
