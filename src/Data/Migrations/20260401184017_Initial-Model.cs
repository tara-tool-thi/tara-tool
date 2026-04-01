using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tara_tool.Migrations
{
    /// <inheritdoc />
    public partial class InitialModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Hash = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectName = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateLastChanged = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessControls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReadAccess = table.Column<bool>(type: "INTEGER", nullable: false),
                    WriteAccess = table.Column<bool>(type: "INTEGER", nullable: false),
                    Manage = table.Column<bool>(type: "INTEGER", nullable: false),
                    Owner = table.Column<bool>(type: "INTEGER", nullable: false),
                    MemberId = table.Column<string>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessControls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessControls_AspNetUsers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessControls_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemDefinitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemName = table.Column<string>(type: "TEXT", nullable: false),
                    ItemFunction = table.Column<string>(type: "TEXT", nullable: false),
                    OperationalEnvironmentText = table.Column<string>(type: "TEXT", nullable: false),
                    PreliminaryArchitectureId = table.Column<long>(type: "INTEGER", nullable: true),
                    ItemBoundaryId = table.Column<long>(type: "INTEGER", nullable: true),
                    OperationalEnvironmentImageId = table.Column<long>(type: "INTEGER", nullable: true),
                    IdProject = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemDefinitions_Images_ItemBoundaryId",
                        column: x => x.ItemBoundaryId,
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemDefinitions_Images_OperationalEnvironmentImageId",
                        column: x => x.OperationalEnvironmentImageId,
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemDefinitions_Images_PreliminaryArchitectureId",
                        column: x => x.PreliminaryArchitectureId,
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemDefinitions_Projects_IdProject",
                        column: x => x.IdProject,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessControls_MemberId",
                table: "AccessControls",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessControls_ProjectId",
                table: "AccessControls",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinitions_IdProject",
                table: "ItemDefinitions",
                column: "IdProject");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinitions_ItemBoundaryId",
                table: "ItemDefinitions",
                column: "ItemBoundaryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinitions_OperationalEnvironmentImageId",
                table: "ItemDefinitions",
                column: "OperationalEnvironmentImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDefinitions_PreliminaryArchitectureId",
                table: "ItemDefinitions",
                column: "PreliminaryArchitectureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessControls");

            migrationBuilder.DropTable(
                name: "ItemDefinitions");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
