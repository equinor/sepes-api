using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class SandboxResourcemadeDeletedNotRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Deleted",
                table: "SandboxResources",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Deleted",
                table: "SandboxResources",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
