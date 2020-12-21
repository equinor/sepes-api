using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class RenameSandboxResourceToCloudResourcePluralized : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CloudResource_Sandboxes_SandboxId",
                table: "CloudResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CloudResource_Studies_StudyId",
                table: "CloudResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CloudResourceOperation_CloudResource_CloudResourceId",
                table: "CloudResourceOperation");

            migrationBuilder.DropForeignKey(
                name: "FK_CloudResourceOperation_CloudResourceOperation_DependsOnOperationId",
                table: "CloudResourceOperation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CloudResourceOperation",
                table: "CloudResourceOperation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CloudResource",
                table: "CloudResource");

            migrationBuilder.RenameTable(
                name: "CloudResourceOperation",
                newName: "CloudResourceOperations");

            migrationBuilder.RenameTable(
                name: "CloudResource",
                newName: "CloudResources");

            migrationBuilder.RenameIndex(
                name: "IX_CloudResourceOperation_DependsOnOperationId",
                table: "CloudResourceOperations",
                newName: "IX_CloudResourceOperations_DependsOnOperationId");

            migrationBuilder.RenameIndex(
                name: "IX_CloudResourceOperation_CloudResourceId",
                table: "CloudResourceOperations",
                newName: "IX_CloudResourceOperations_CloudResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_CloudResource_StudyId",
                table: "CloudResources",
                newName: "IX_CloudResources_StudyId");

            migrationBuilder.RenameIndex(
                name: "IX_CloudResource_SandboxId",
                table: "CloudResources",
                newName: "IX_CloudResources_SandboxId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CloudResourceOperations",
                table: "CloudResourceOperations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CloudResources",
                table: "CloudResources",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResourceOperations_CloudResources_CloudResourceId",
                table: "CloudResourceOperations",
                column: "CloudResourceId",
                principalTable: "CloudResources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResourceOperations_CloudResourceOperations_DependsOnOperationId",
                table: "CloudResourceOperations",
                column: "DependsOnOperationId",
                principalTable: "CloudResourceOperations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResources_Sandboxes_SandboxId",
                table: "CloudResources",
                column: "SandboxId",
                principalTable: "Sandboxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResources_Studies_StudyId",
                table: "CloudResources",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CloudResourceOperations_CloudResources_CloudResourceId",
                table: "CloudResourceOperations");

            migrationBuilder.DropForeignKey(
                name: "FK_CloudResourceOperations_CloudResourceOperations_DependsOnOperationId",
                table: "CloudResourceOperations");

            migrationBuilder.DropForeignKey(
                name: "FK_CloudResources_Sandboxes_SandboxId",
                table: "CloudResources");

            migrationBuilder.DropForeignKey(
                name: "FK_CloudResources_Studies_StudyId",
                table: "CloudResources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CloudResources",
                table: "CloudResources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CloudResourceOperations",
                table: "CloudResourceOperations");

            migrationBuilder.RenameTable(
                name: "CloudResources",
                newName: "CloudResource");

            migrationBuilder.RenameTable(
                name: "CloudResourceOperations",
                newName: "CloudResourceOperation");

            migrationBuilder.RenameIndex(
                name: "IX_CloudResources_StudyId",
                table: "CloudResource",
                newName: "IX_CloudResource_StudyId");

            migrationBuilder.RenameIndex(
                name: "IX_CloudResources_SandboxId",
                table: "CloudResource",
                newName: "IX_CloudResource_SandboxId");

            migrationBuilder.RenameIndex(
                name: "IX_CloudResourceOperations_DependsOnOperationId",
                table: "CloudResourceOperation",
                newName: "IX_CloudResourceOperation_DependsOnOperationId");

            migrationBuilder.RenameIndex(
                name: "IX_CloudResourceOperations_CloudResourceId",
                table: "CloudResourceOperation",
                newName: "IX_CloudResourceOperation_CloudResourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CloudResource",
                table: "CloudResource",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CloudResourceOperation",
                table: "CloudResourceOperation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResource_Sandboxes_SandboxId",
                table: "CloudResource",
                column: "SandboxId",
                principalTable: "Sandboxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResource_Studies_StudyId",
                table: "CloudResource",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResourceOperation_CloudResource_CloudResourceId",
                table: "CloudResourceOperation",
                column: "CloudResourceId",
                principalTable: "CloudResource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResourceOperation_CloudResourceOperation_DependsOnOperationId",
                table: "CloudResourceOperation",
                column: "DependsOnOperationId",
                principalTable: "CloudResourceOperation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
