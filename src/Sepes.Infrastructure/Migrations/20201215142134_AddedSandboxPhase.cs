using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class AddedSandboxPhase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SandboxPhaseHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    SandboxId = table.Column<int>(nullable: false),
                    Counter = table.Column<int>(nullable: false),
                    Phase = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SandboxPhaseHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SandboxPhaseHistory_Sandboxes_SandboxId",
                        column: x => x.SandboxId,
                        principalTable: "Sandboxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SandboxPhaseHistory_SandboxId",
                table: "SandboxPhaseHistory",
                column: "SandboxId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SandboxPhaseHistory");
        }
    }
}
