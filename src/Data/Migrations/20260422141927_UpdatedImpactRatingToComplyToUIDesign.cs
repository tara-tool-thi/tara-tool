using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedImpactRatingToComplyToUIDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImpactCategory",
                table: "ImpactRatings");

            migrationBuilder.AddColumn<int>(
                name: "FinancialImpact",
                table: "ImpactRatings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OperationalImpact",
                table: "ImpactRatings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrivacyImpact",
                table: "ImpactRatings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SafetyImpact",
                table: "ImpactRatings",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinancialImpact",
                table: "ImpactRatings");

            migrationBuilder.DropColumn(
                name: "OperationalImpact",
                table: "ImpactRatings");

            migrationBuilder.DropColumn(
                name: "PrivacyImpact",
                table: "ImpactRatings");

            migrationBuilder.DropColumn(
                name: "SafetyImpact",
                table: "ImpactRatings");

            migrationBuilder.AddColumn<int>(
                name: "ImpactCategory",
                table: "ImpactRatings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
