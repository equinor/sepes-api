using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class CloudResourceAddDeletedBoolField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "CloudResources",
                nullable: true);

            migrationBuilder.Sql("UPDATE dbo.CloudResources SET Deleted = 1 WHERE DeletedAt IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "CloudResources");
        }
    }
}
