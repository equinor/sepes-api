using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class AddResourceRoleAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CloudResourceRoleAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CloudResourceId = table.Column<int>(nullable: false),
                    UserOjectId = table.Column<string>(maxLength: 64, nullable: true),
                    RoleId = table.Column<string>(maxLength: 512, nullable: true),
                    ForeignSystemId = table.Column<string>(maxLength: 512, nullable: true),
                    Deleted = table.Column<bool>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 64, nullable: true),
                    DeletedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudResourceRoleAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloudResourceRoleAssignments_CloudResources_CloudResourceId",
                        column: x => x.CloudResourceId,
                        principalTable: "CloudResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CloudResourceRoleAssignments_CloudResourceId",
                table: "CloudResourceRoleAssignments",
                column: "CloudResourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CloudResourceRoleAssignments");
        }
    }
}
