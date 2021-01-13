using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class AddedDiskPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiskSizes",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Size = table.Column<int>(nullable: false),
                    DisplayText = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiskSizes", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "RegionDiskSize",
                columns: table => new
                {
                    RegionKey = table.Column<string>(nullable: false),
                    VmDiskKey = table.Column<string>(nullable: false),
                    Price = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionDiskSize", x => new { x.RegionKey, x.VmDiskKey });
                    table.ForeignKey(
                        name: "FK_RegionDiskSize_Regions_RegionKey",
                        column: x => x.RegionKey,
                        principalTable: "Regions",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegionDiskSize_DiskSizes_VmDiskKey",
                        column: x => x.VmDiskKey,
                        principalTable: "DiskSizes",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegionDiskSize_VmDiskKey",
                table: "RegionDiskSize",
                column: "VmDiskKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegionDiskSize");

            migrationBuilder.DropTable(
                name: "DiskSizes");
        }
    }
}
