using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class StudyIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Studies_Closed_Restricted",
                table: "Studies",
                columns: new[] { "Closed", "Restricted" })
                .Annotation("SqlServer:Include", new[] { "Id", "Name", "Description", "Vendor", "LogoUrl" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Studies_Closed_Restricted",
                table: "Studies");
        }
    }
}
