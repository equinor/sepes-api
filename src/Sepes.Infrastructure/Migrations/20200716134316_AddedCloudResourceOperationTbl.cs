using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class AddedCloudResourceOperationTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedFromAzure",
                table: "CloudResources");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sandboxes",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechnicalContactEmail",
                table: "Sandboxes",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TechnicalContactName",
                table: "Sandboxes",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Deleted",
                table: "CloudResources",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SandboxId",
                table: "CloudResources",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CloudResourceOperation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    CloudResourceId = table.Column<int>(nullable: false),
                    JobType = table.Column<string>(nullable: true),
                    InProgressWith = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloudResourceOperation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloudResourceOperation_CloudResources_CloudResourceId",
                        column: x => x.CloudResourceId,
                        principalTable: "CloudResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CloudResources_SandboxId",
                table: "CloudResources",
                column: "SandboxId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudResourceOperation_CloudResourceId",
                table: "CloudResourceOperation",
                column: "CloudResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResources_Sandboxes_SandboxId",
                table: "CloudResources",
                column: "SandboxId",
                principalTable: "Sandboxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CloudResources_Sandboxes_SandboxId",
                table: "CloudResources");

            migrationBuilder.DropTable(
                name: "CloudResourceOperation");

            migrationBuilder.DropIndex(
                name: "IX_CloudResources_SandboxId",
                table: "CloudResources");

            migrationBuilder.DropColumn(
                name: "TechnicalContactEmail",
                table: "Sandboxes");

            migrationBuilder.DropColumn(
                name: "TechnicalContactName",
                table: "Sandboxes");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "CloudResources");

            migrationBuilder.DropColumn(
                name: "SandboxId",
                table: "CloudResources");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sandboxes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedFromAzure",
                table: "CloudResources",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
