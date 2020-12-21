using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class RenameSandboxResourceForeignKeyds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CloudResourceOperation_CloudResource_SandboxResourceId",
                table: "CloudResourceOperation");

            migrationBuilder.DropIndex(
                name: "IX_CloudResourceOperation_SandboxResourceId",
                table: "CloudResourceOperation");

            migrationBuilder.AddColumn<int>(
               name: "CloudResourceId",
               table: "CloudResourceOperation",
               nullable: false,
               defaultValue: 0);

            migrationBuilder.Sql("UPDATE [dbo].[CloudResourceOperation] SET [CloudResourceId] = [SandboxResourceId]");

            migrationBuilder.DropColumn(
                name: "SandboxResourceId",
                table: "CloudResourceOperation");           

            migrationBuilder.CreateIndex(
                name: "IX_CloudResourceOperation_CloudResourceId",
                table: "CloudResourceOperation",
                column: "CloudResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResourceOperation_CloudResource_CloudResourceId",
                table: "CloudResourceOperation",
                column: "CloudResourceId",
                principalTable: "CloudResource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CloudResourceOperation_CloudResource_CloudResourceId",
                table: "CloudResourceOperation");

            migrationBuilder.DropIndex(
                name: "IX_CloudResourceOperation_CloudResourceId",
                table: "CloudResourceOperation");

            migrationBuilder.AddColumn<int>(
                name: "SandboxResourceId",
                table: "CloudResourceOperation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE [dbo].[CloudResourceOperation] SET [SandboxResourceId] = [CloudResourceId]");

            migrationBuilder.DropColumn(
                name: "CloudResourceId",
                table: "CloudResourceOperation");

            migrationBuilder.CreateIndex(
                name: "IX_CloudResourceOperation_SandboxResourceId",
                table: "CloudResourceOperation",
                column: "SandboxResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResourceOperation_CloudResource_SandboxResourceId",
                table: "CloudResourceOperation",
                column: "SandboxResourceId",
                principalTable: "CloudResource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
