using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class DamageScenarioUpdate4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Financial",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Operational",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Privacy",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Safety",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Financial",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "Operational",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "Privacy",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "Safety",
                table: "DamageScenarios");
        }
    }
}
