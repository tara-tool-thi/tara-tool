using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class Fixtag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Projects_IdProject",
                table: "Tags");

            migrationBuilder.RenameColumn(
                name: "IdProject",
                table: "Tags",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_IdProject",
                table: "Tags",
                newName: "IX_Tags_ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Projects_ProjectId",
                table: "Tags",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Projects_ProjectId",
                table: "Tags");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Tags",
                newName: "IdProject");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_ProjectId",
                table: "Tags",
                newName: "IX_Tags_IdProject");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Projects_IdProject",
                table: "Tags",
                column: "IdProject",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
