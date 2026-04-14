using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

// Part of the Require_AccessControl Migration -> I do net know if ef core
// requires this Empty Migration, because no Data was added. Due to the fact,
// that we always have a Migration.cs and a Migration.Designer.cs I will leave
// this empty Migration, so it is consistent with all the other ones - ardwetha
namespace tara_tool.Migrations {
/// <inheritdoc />
public partial class Require_AccessControl : Migration {
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder) {}

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder) {}
}
}
