using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class DamageScenarioUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "financial",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "financial",
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
        }
    }
}
