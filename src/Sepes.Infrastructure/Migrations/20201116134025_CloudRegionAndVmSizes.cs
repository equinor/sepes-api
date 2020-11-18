using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class CloudRegionAndVmSizes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Disabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "VmSizes",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Category = table.Column<string>(nullable: true),
                    NumberOfCores = table.Column<int>(nullable: false),
                    OsDiskSizeInMB = table.Column<int>(nullable: false),
                    ResourceDiskSizeInMB = table.Column<int>(nullable: false),
                    MemoryGB = table.Column<int>(nullable: false),
                    MaxDataDiskCount = table.Column<int>(nullable: false),
                    MaxNetworkInterfaces = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VmSizes", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "RegionVmSize",
                columns: table => new
                {
                    RegionKey = table.Column<string>(nullable: false),
                    VmSizeKey = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionVmSize", x => new { x.RegionKey, x.VmSizeKey });
                    table.ForeignKey(
                        name: "FK_RegionVmSize_Regions_RegionKey",
                        column: x => x.RegionKey,
                        principalTable: "Regions",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegionVmSize_VmSizes_VmSizeKey",
                        column: x => x.VmSizeKey,
                        principalTable: "VmSizes",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegionVmSize_VmSizeKey",
                table: "RegionVmSize",
                column: "VmSizeKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegionVmSize");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropTable(
                name: "VmSizes");
        }
    }
}
