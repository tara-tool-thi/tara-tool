using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class AddItemDefinitionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemBoundaryText",
                table: "ItemDefinitions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ItemDescription",
                table: "ItemDefinitions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreliminaryArchitectureText",
                table: "ItemDefinitions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "TechnicalSketchId",
                table: "ItemDefinitions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinitions_TechnicalSketchId",
                table: "ItemDefinitions",
                column: "TechnicalSketchId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDefinitions_Images_TechnicalSketchId",
                table: "ItemDefinitions",
                column: "TechnicalSketchId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemDefinitions_Images_TechnicalSketchId",
                table: "ItemDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_ItemDefinitions_TechnicalSketchId",
                table: "ItemDefinitions");

            migrationBuilder.DropColumn(
                name: "ItemBoundaryText",
                table: "ItemDefinitions");

            migrationBuilder.DropColumn(
                name: "ItemDescription",
                table: "ItemDefinitions");

            migrationBuilder.DropColumn(
                name: "PreliminaryArchitectureText",
                table: "ItemDefinitions");

            migrationBuilder.DropColumn(
                name: "TechnicalSketchId",
                table: "ItemDefinitions");
        }
    }
}
