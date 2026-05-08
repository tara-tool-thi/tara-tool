using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Assets_AssetId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "AssetItemDefinition");

            migrationBuilder.DropIndex(
                name: "IX_Tags_AssetId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "Tags");

            migrationBuilder.AddColumn<long>(
                name: "IdProject",
                table: "Tags",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Assets",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "IdItemDefinition",
                table: "Assets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "IdTag",
                table: "Assets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TagId",
                table: "Assets",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_IdProject",
                table: "Tags",
                column: "IdProject");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_IdItemDefinition",
                table: "Assets",
                column: "IdItemDefinition");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_TagId",
                table: "Assets",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_ItemDefinitions_IdItemDefinition",
                table: "Assets",
                column: "IdItemDefinition",
                principalTable: "ItemDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Tags_TagId",
                table: "Assets",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Projects_IdProject",
                table: "Tags",
                column: "IdProject",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_ItemDefinitions_IdItemDefinition",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Tags_TagId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Projects_IdProject",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_IdProject",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Assets_IdItemDefinition",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_TagId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "IdProject",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "IdItemDefinition",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "IdTag",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Assets");

            migrationBuilder.AddColumn<long>(
                name: "AssetId",
                table: "Tags",
                type: "INTEGER",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Tags_AssetId",
                table: "Tags",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetItemDefinition_ItemDefinitionsId",
                table: "AssetItemDefinition",
                column: "ItemDefinitionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Assets_AssetId",
                table: "Tags",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");
        }
    }
}
