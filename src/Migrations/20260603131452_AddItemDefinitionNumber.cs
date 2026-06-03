using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class AddItemDefinitionNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ItemNumber",
                table: "ItemDefinitions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.Sql(
                """
                UPDATE ItemDefinitions
                SET ItemNumber = (
                    SELECT NumberedItems.ItemNumber
                    FROM (
                        SELECT Id,
                               ROW_NUMBER() OVER (PARTITION BY IdProject ORDER BY Id) AS ItemNumber
                        FROM ItemDefinitions
                    ) AS NumberedItems
                    WHERE NumberedItems.Id = ItemDefinitions.Id
                )
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemNumber",
                table: "ItemDefinitions");
        }
    }
}
