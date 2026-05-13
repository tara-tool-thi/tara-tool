using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class AddAttackSteps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Steps",
                table: "AttackPaths");

            migrationBuilder.CreateTable(
                name: "AttackSteps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    AttackPathId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttackSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttackSteps_AttackPaths_AttackPathId",
                        column: x => x.AttackPathId,
                        principalTable: "AttackPaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttackSteps_AttackPathId",
                table: "AttackSteps",
                column: "AttackPathId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttackSteps");

            migrationBuilder.AddColumn<string>(
                name: "Steps",
                table: "AttackPaths",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
