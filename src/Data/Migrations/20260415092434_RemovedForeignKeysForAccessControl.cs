using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class RemovedForeignKeysForAccessControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessControls_AspNetUsers_MemberId",
                table: "AccessControls");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "AccessControls",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessControls_MemberId",
                table: "AccessControls",
                newName: "IX_AccessControls_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessControls_AspNetUsers_ApplicationUserId",
                table: "AccessControls",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessControls_AspNetUsers_ApplicationUserId",
                table: "AccessControls");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "AccessControls",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessControls_ApplicationUserId",
                table: "AccessControls",
                newName: "IX_AccessControls_MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessControls_AspNetUsers_MemberId",
                table: "AccessControls",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
