using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class RenameSandboxResourceToCloudResourceAlsoOperationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SandboxResourceOperations_SandboxResourceOperations_DependsOnOperationId",
                table: "SandboxResourceOperations");

            migrationBuilder.DropForeignKey(
                name: "FK_SandboxResourceOperations_SandboxResources_SandboxResourceId",
                table: "SandboxResourceOperations");

            migrationBuilder.DropForeignKey(
                name: "FK_SandboxResources_Sandboxes_SandboxId",
                table: "SandboxResources");

            migrationBuilder.DropForeignKey(
                name: "FK_SandboxResources_Studies_StudyId",
                table: "SandboxResources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SandboxResources",
                table: "SandboxResources");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SandboxResourceOperations",
                table: "SandboxResourceOperations");

            migrationBuilder.RenameTable(
                name: "SandboxResources",
                newName: "CloudResource");

            migrationBuilder.RenameTable(
                name: "SandboxResourceOperations",
                newName: "CloudResourceOperation");

            migrationBuilder.RenameIndex(
                name: "IX_SandboxResources_StudyId",
                table: "CloudResource",
                newName: "IX_CloudResource_StudyId");

            migrationBuilder.RenameIndex(
                name: "IX_SandboxResources_SandboxId",
                table: "CloudResource",
                newName: "IX_CloudResource_SandboxId");

            migrationBuilder.RenameIndex(
                name: "IX_SandboxResourceOperations_SandboxResourceId",
                table: "CloudResourceOperation",
                newName: "IX_CloudResourceOperation_SandboxResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_SandboxResourceOperations_DependsOnOperationId",
                table: "CloudResourceOperation",
                newName: "IX_CloudResourceOperation_DependsOnOperationId");

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
                name: "FK_CloudResourceOperation_CloudResourceOperation_DependsOnOperationId",
                table: "CloudResourceOperation",
                column: "DependsOnOperationId",
                principalTable: "CloudResourceOperation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResourceOperation_CloudResource_SandboxResourceId",
                table: "CloudResourceOperation",
                column: "SandboxResourceId",
                principalTable: "CloudResource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CloudResource_Sandboxes_SandboxId",
                table: "CloudResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CloudResource_Studies_StudyId",
                table: "CloudResource");

            migrationBuilder.DropForeignKey(
                name: "FK_CloudResourceOperation_CloudResourceOperation_DependsOnOperationId",
                table: "CloudResourceOperation");

            migrationBuilder.DropForeignKey(
                name: "FK_CloudResourceOperation_CloudResource_SandboxResourceId",
                table: "CloudResourceOperation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CloudResourceOperation",
                table: "CloudResourceOperation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CloudResource",
                table: "CloudResource");

            migrationBuilder.RenameTable(
                name: "CloudResourceOperation",
                newName: "SandboxResourceOperations");

            migrationBuilder.RenameTable(
                name: "CloudResource",
                newName: "SandboxResources");

            migrationBuilder.RenameIndex(
                name: "IX_CloudResourceOperation_SandboxResourceId",
                table: "SandboxResourceOperations",
                newName: "IX_SandboxResourceOperations_SandboxResourceId");

            migrationBuilder.RenameIndex(
                name: "IX_CloudResourceOperation_DependsOnOperationId",
                table: "SandboxResourceOperations",
                newName: "IX_SandboxResourceOperations_DependsOnOperationId");

            migrationBuilder.RenameIndex(
                name: "IX_CloudResource_StudyId",
                table: "SandboxResources",
                newName: "IX_SandboxResources_StudyId");

            migrationBuilder.RenameIndex(
                name: "IX_CloudResource_SandboxId",
                table: "SandboxResources",
                newName: "IX_SandboxResources_SandboxId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SandboxResourceOperations",
                table: "SandboxResourceOperations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SandboxResources",
                table: "SandboxResources",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SandboxResourceOperations_SandboxResourceOperations_DependsOnOperationId",
                table: "SandboxResourceOperations",
                column: "DependsOnOperationId",
                principalTable: "SandboxResourceOperations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
        }
    }
}
