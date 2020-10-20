using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class ResourceOpsMaxTryCOunt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxTryCount",
                table: "SandboxResourceOperations",
                nullable: false,
                defaultValue: 3);           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxTryCount",
                table: "SandboxResourceOperations");
        }
    }
}
