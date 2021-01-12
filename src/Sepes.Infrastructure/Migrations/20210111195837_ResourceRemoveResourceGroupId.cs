using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class ResourceRemoveResourceGroupId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceGroupId",
                table: "CloudResources");

            migrationBuilder.AddColumn<int>(
                name: "ParentResourceId",
                table: "CloudResources",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CloudResources_ParentResourceId",
                table: "CloudResources",
                column: "ParentResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResources_CloudResources_ParentResourceId",
                table: "CloudResources",
                column: "ParentResourceId",
                principalTable: "CloudResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CloudResources_CloudResources_ParentResourceId",
                table: "CloudResources");

            migrationBuilder.DropIndex(
                name: "IX_CloudResources_ParentResourceId",
                table: "CloudResources");

            migrationBuilder.DropColumn(
                name: "ParentResourceId",
                table: "CloudResources");

            migrationBuilder.AddColumn<string>(
                name: "ResourceGroupId",
                table: "CloudResources",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
