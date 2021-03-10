using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class MoreIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CloudResources_SandboxId",
                table: "CloudResources");

            migrationBuilder.DropIndex(
                name: "IX_CloudResources_StudyId",
                table: "CloudResources");

            migrationBuilder.CreateIndex(
                name: "IX_CloudResources_SandboxId_ResourceName",
                table: "CloudResources",
                columns: new[] { "SandboxId", "ResourceName" },
                filter: "[Deleted] = 0")
                .Annotation("SqlServer:Include", new[] { "Id", "Region" });

            migrationBuilder.CreateIndex(
                name: "IX_CloudResources_StudyId_ResourceName",
                table: "CloudResources",
                columns: new[] { "StudyId", "ResourceName" },
                filter: "[Deleted] = 0")
                .Annotation("SqlServer:Include", new[] { "Id", "Region" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CloudResources_SandboxId_ResourceName",
                table: "CloudResources");

            migrationBuilder.DropIndex(
                name: "IX_CloudResources_StudyId_ResourceName",
                table: "CloudResources");

            migrationBuilder.CreateIndex(
                name: "IX_CloudResources_SandboxId",
                table: "CloudResources",
                column: "SandboxId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudResources_StudyId",
                table: "CloudResources",
                column: "StudyId");
        }
    }
}
