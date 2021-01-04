using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class MovePriceFieldToRegionVmSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "VmSizes");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "RegionVmSize",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "RegionVmSize");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "VmSizes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
