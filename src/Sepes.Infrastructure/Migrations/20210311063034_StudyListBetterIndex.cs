using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class StudyListBetterIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Studies_Closed",
                table: "Studies",
                column: "Closed")
                .Annotation("SqlServer:Include", new[] { "Id", "Restricted", "Name", "Description", "Vendor", "LogoUrl" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Studies_Closed",
                table: "Studies");
        }
    }
}
