using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class DamageScenarioUpdate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AvailabilityAffected",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConfidentialityAffected",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IntegrityAffected",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailabilityAffected",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "ConfidentialityAffected",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "IntegrityAffected",
                table: "DamageScenarios");
        }
    }
}
