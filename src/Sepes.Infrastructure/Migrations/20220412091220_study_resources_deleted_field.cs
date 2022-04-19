using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sepes.Infrastructure.Migrations
{
    public partial class study_resources_deleted_field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsResourcesDeleted",
                table: "Studies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsResourcesDeleted",
                table: "Studies");
        }
    }
}
