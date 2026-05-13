using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToThreatSceanrioAndAttackPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ThreatScenarios",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AttackPaths",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "RiskTreatmentBool",
                table: "AttackPaths",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RiskTreatmentText",
                table: "AttackPaths",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Steps",
                table: "AttackPaths",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ThreatScenarios");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AttackPaths");

            migrationBuilder.DropColumn(
                name: "RiskTreatmentBool",
                table: "AttackPaths");

            migrationBuilder.DropColumn(
                name: "RiskTreatmentText",
                table: "AttackPaths");

            migrationBuilder.DropColumn(
                name: "Steps",
                table: "AttackPaths");
        }
    }
}
