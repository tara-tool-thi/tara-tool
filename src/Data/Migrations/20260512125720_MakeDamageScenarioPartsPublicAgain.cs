using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class MakeDamageScenarioPartsPublicAgain : Migration
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

            migrationBuilder.AddColumn<int>(
                name: "Financial",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IntegrityAffected",
                table: "DamageScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

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
                name: "AvailabilityAffected",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "ConfidentialityAffected",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "Financial",
                table: "DamageScenarios");

            migrationBuilder.DropColumn(
                name: "IntegrityAffected",
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
