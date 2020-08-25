using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Sepes.Infrastructure.Model;

namespace Sepes.Infrastructure.Migrations
{
    public partial class CreateVariablesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SandboxResource_Sandboxes_SandboxId",
                table: "SandboxResource");

            migrationBuilder.DropForeignKey(
                name: "FK_SandboxResource_Studies_StudyId",
                table: "SandboxResource");

            migrationBuilder.DropForeignKey(
                name: "FK_SandboxResourceOperation_SandboxResource_CloudResourceId",
                table: "SandboxResourceOperation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SandboxResourceOperation",
                table: "SandboxResourceOperation");

            migrationBuilder.DropIndex(
                name: "IX_SandboxResourceOperation_CloudResourceId",
                table: "SandboxResourceOperation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SandboxResource",
                table: "SandboxResource");

            migrationBuilder.DropColumn(
                name: "CloudResourceId",
                table: "SandboxResourceOperation");

            migrationBuilder.DropColumn(
                name: "InProgressWith",
                table: "SandboxResourceOperation");

            migrationBuilder.DropColumn(
                name: "JobType",
                table: "SandboxResourceOperation");

            migrationBuilder.RenameTable(
                name: "SandboxResourceOperation",
                newName: "SandboxResourceOperations");

            migrationBuilder.RenameTable(
                name: "SandboxResource",
                newName: "SandboxResources");

            migrationBuilder.RenameIndex(
                name: "IX_SandboxResource_StudyId",
                table: "SandboxResources",
                newName: "IX_SandboxResources_StudyId");

            migrationBuilder.RenameIndex(
                name: "IX_SandboxResource_SandboxId",
                table: "SandboxResources",
                newName: "IX_SandboxResources_SandboxId");

            migrationBuilder.AddColumn<int>(
                name: "SandboxResourceId",
                table: "SandboxResourceOperations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "SandboxResourceOperations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SandboxResourceOperations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TryCount",
                table: "SandboxResourceOperations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SandboxResourceOperations",
                table: "SandboxResourceOperations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SandboxResources",
                table: "SandboxResources",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Variables",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Int1 = table.Column<int>(nullable: true),
                    Int2 = table.Column<int>(nullable: true),
                    Int3 = table.Column<int>(nullable: true),
                    Str1 = table.Column<string>(nullable: true),
                    Str2 = table.Column<string>(nullable: true),
                    Str3 = table.Column<string>(nullable: true),
                    Bool1 = table.Column<bool>(nullable: true),
                    Bool2 = table.Column<bool>(nullable: true),
                    Bool3 = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variables", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SandboxResourceOperations_SandboxResourceId",
                table: "SandboxResourceOperations",
                column: "SandboxResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SandboxResourceOperations_SandboxResources_SandboxResourceId",
                table: "SandboxResourceOperations",
                column: "SandboxResourceId",
                principalTable: "SandboxResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SandboxResources_Sandboxes_SandboxId",
                table: "SandboxResources",
                column: "SandboxId",
                principalTable: "Sandboxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SandboxResources_Studies_StudyId",
                table: "SandboxResources",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql($"INSERT INTO Variables (Name, Description, Int1, Int2, Created, CreatedBy, Updated, UpdatedBy) VALUES ('{VariableNames.BastionTimeoutAndRetryCount}', 'Int1 is timeout in seconds, Int2 is max retry count', 600, 2, getutcdate(), 'Migration', getutcdate(), 'Migration')");
            migrationBuilder.Sql($"INSERT INTO Variables (Name, Description, Int1, Int2, Created, CreatedBy, Updated, UpdatedBy) VALUES ('{VariableNames.VmTimeoutAndRetryCount}', 'Int1 is timeout in seconds, Int2 is max retry count', 600, 2, getutcdate(), 'Migration', getutcdate(), 'Migration')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SandboxResourceOperations_SandboxResources_SandboxResourceId",
                table: "SandboxResourceOperations");

            migrationBuilder.DropForeignKey(
                name: "FK_SandboxResources_Sandboxes_SandboxId",
                table: "SandboxResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SandboxResources_Studies_StudyId",
                table: "SandboxResources");

            migrationBuilder.DropTable(
                name: "Variables");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SandboxResources",
                table: "SandboxResources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SandboxResourceOperations",
                table: "SandboxResourceOperations");

            migrationBuilder.DropIndex(
                name: "IX_SandboxResourceOperations_SandboxResourceId",
                table: "SandboxResourceOperations");

            migrationBuilder.DropColumn(
                name: "SandboxResourceId",
                table: "SandboxResourceOperations");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "SandboxResourceOperations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SandboxResourceOperations");

            migrationBuilder.DropColumn(
                name: "TryCount",
                table: "SandboxResourceOperations");

            migrationBuilder.RenameTable(
                name: "SandboxResources",
                newName: "SandboxResource");

            migrationBuilder.RenameTable(
                name: "SandboxResourceOperations",
                newName: "SandboxResourceOperation");

            migrationBuilder.RenameIndex(
                name: "IX_SandboxResources_StudyId",
                table: "SandboxResource",
                newName: "IX_SandboxResource_StudyId");

            migrationBuilder.RenameIndex(
                name: "IX_SandboxResources_SandboxId",
                table: "SandboxResource",
                newName: "IX_SandboxResource_SandboxId");

            migrationBuilder.AddColumn<int>(
                name: "CloudResourceId",
                table: "SandboxResourceOperation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InProgressWith",
                table: "SandboxResourceOperation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobType",
                table: "SandboxResourceOperation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SandboxResource",
                table: "SandboxResource",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SandboxResourceOperation",
                table: "SandboxResourceOperation",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SandboxResourceOperation_CloudResourceId",
                table: "SandboxResourceOperation",
                column: "CloudResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SandboxResource_Sandboxes_SandboxId",
                table: "SandboxResource",
                column: "SandboxId",
                principalTable: "Sandboxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SandboxResource_Studies_StudyId",
                table: "SandboxResource",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SandboxResourceOperation_SandboxResource_CloudResourceId",
                table: "SandboxResourceOperation",
                column: "CloudResourceId",
                principalTable: "SandboxResource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
