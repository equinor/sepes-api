using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class SandboxResourceLastKnownProvisioningState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastKnownProvisioningState",
                table: "SandboxResources",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastKnownProvisioningState",
                table: "SandboxResources");
        }
    }
}
