using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class VmImagev2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Studies",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 4096,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "VmImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    ForeignSystemId = table.Column<string>(maxLength: 1024, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VmImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VmImageSearchProperties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Publisher = table.Column<string>(nullable: true),
                    Offer = table.Column<string>(nullable: true),
                    Sku = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VmImageSearchProperties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionVmImage",
                columns: table => new
                {
                    RegionKey = table.Column<string>(maxLength: 64, nullable: false),
                    VmImageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionVmImage", x => new { x.RegionKey, x.VmImageId });
                    table.ForeignKey(
                        name: "FK_RegionVmImage_Regions_RegionKey",
                        column: x => x.RegionKey,
                        principalTable: "Regions",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegionVmImage_VmImages_VmImageId",
                        column: x => x.VmImageId,
                        principalTable: "VmImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegionVmImage_VmImageId",
                table: "RegionVmImage",
                column: "VmImageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegionVmImage");

            migrationBuilder.DropTable(
                name: "VmImageSearchProperties");

            migrationBuilder.DropTable(
                name: "VmImages");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Studies",
                type: "nvarchar(max)",
                maxLength: 4096,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);
        }
    }
}
