using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class StudyIndex1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Studies_Closed_Restricted",
                table: "Studies");

            migrationBuilder.CreateIndex(
                name: "IX_Studies_Id_Restricted",
                table: "Studies",
                columns: new[] { "Id", "Restricted" },
                filter: "[Closed] = 0")
                .Annotation("SqlServer:Include", new[] { "Name", "Description", "Vendor", "LogoUrl" });

            migrationBuilder.CreateIndex(
                name: "IX_Sandboxes_Id_StudyId",
                table: "Sandboxes",
                columns: new[] { "Id", "StudyId" },
                filter: "[Deleted] = 0")
                .Annotation("SqlServer:Include", new[] { "Name", "Region" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Studies_Id_Restricted",
                table: "Studies");

            migrationBuilder.DropIndex(
                name: "IX_Sandboxes_Id_StudyId",
                table: "Sandboxes");

            migrationBuilder.CreateIndex(
                name: "IX_Studies_Closed_Restricted",
                table: "Studies",
                columns: new[] { "Closed", "Restricted" })
                .Annotation("SqlServer:Include", new[] { "Id", "Name", "Description", "Vendor", "LogoUrl" });
        }
    }
}
