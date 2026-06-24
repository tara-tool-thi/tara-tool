using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class ThreatScenarioRemoveSTRIDE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StrideCategorie",
                table: "ThreatScenarios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StrideCategorie",
                table: "ThreatScenarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
