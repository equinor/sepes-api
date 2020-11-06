using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class SandboxDatasets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubOperationType",
                table: "SandboxResourceOperations");

            migrationBuilder.DropColumn(
                name: "UpdateConfigString",
                table: "SandboxResourceOperations");

            migrationBuilder.CreateTable(
                name: "SandboxDatasets",
                columns: table => new
                {
                    SandboxId = table.Column<int>(nullable: false),
                    DatasetId = table.Column<int>(nullable: false),
                    AddedBy = table.Column<string>(nullable: true),
                    Added = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SandboxDatasets", x => new { x.SandboxId, x.DatasetId });
                    table.ForeignKey(
                        name: "FK_SandboxDatasets_Datasets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "Datasets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SandboxDatasets_Sandboxes_SandboxId",
                        column: x => x.SandboxId,
                        principalTable: "Sandboxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SandboxDatasets_DatasetId",
                table: "SandboxDatasets",
                column: "DatasetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SandboxDatasets");

            migrationBuilder.AddColumn<string>(
                name: "SubOperationType",
                table: "SandboxResourceOperations",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdateConfigString",
                table: "SandboxResourceOperations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
