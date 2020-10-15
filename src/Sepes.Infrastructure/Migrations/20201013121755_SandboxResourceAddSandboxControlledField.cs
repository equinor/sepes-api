using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class SandboxResourceAddSandboxControlledField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<bool>(
                name: "SandboxControlled",
                table: "SandboxResources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE [dbo].[SandboxResources] SET [SandboxControlled] = 1 WHERE ResourceType NOT LIKE 'VirtualMachine'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SandboxControlled",
                table: "SandboxResources");
        }
    }
}
