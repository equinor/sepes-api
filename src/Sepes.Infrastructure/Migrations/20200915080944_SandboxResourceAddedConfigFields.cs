using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class SandboxResourceAddedConfigFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "SandboxResourceOperations");

            migrationBuilder.AddColumn<string>(
                name: "ConfigString",
                table: "SandboxResources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "SandboxResources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "SandboxResources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarriedOutBySessionId",
                table: "SandboxResourceOperations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBySessionId",
                table: "SandboxResourceOperations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfigString",
                table: "SandboxResources");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "SandboxResources");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "SandboxResources");

            migrationBuilder.DropColumn(
                name: "CarriedOutBySessionId",
                table: "SandboxResourceOperations");

            migrationBuilder.DropColumn(
                name: "CreatedBySessionId",
                table: "SandboxResourceOperations");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "SandboxResourceOperations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
