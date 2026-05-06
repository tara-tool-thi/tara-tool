using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class FixeRelationTagAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Tags_TagId",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_TagId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Assets");

            migrationBuilder.AlterColumn<long>(
                name: "IdTag",
                table: "Assets",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_IdTag",
                table: "Assets",
                column: "IdTag");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Tags_IdTag",
                table: "Assets",
                column: "IdTag",
                principalTable: "Tags",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Tags_IdTag",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_IdTag",
                table: "Assets");

            migrationBuilder.AlterColumn<long>(
                name: "IdTag",
                table: "Assets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TagId",
                table: "Assets",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_TagId",
                table: "Assets",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Tags_TagId",
                table: "Assets",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");
        }
    }
}
