using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class VmImagevCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "VmImageSearchProperties",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "VmImages",
                maxLength: 64,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "VmImageSearchProperties");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "VmImages");
        }
    }
}
