using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class SandboxOperationsAddDependentOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DependsOnOperationId",
                table: "SandboxResourceOperations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SandboxResourceOperations_DependsOnOperationId",
                table: "SandboxResourceOperations",
                column: "DependsOnOperationId");

            migrationBuilder.AddForeignKey(
                name: "FK_SandboxResourceOperations_SandboxResourceOperations_DependsOnOperationId",
                table: "SandboxResourceOperations",
                column: "DependsOnOperationId",
                principalTable: "SandboxResourceOperations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SandboxResourceOperations_SandboxResourceOperations_DependsOnOperationId",
                table: "SandboxResourceOperations");

            migrationBuilder.DropIndex(
                name: "IX_SandboxResourceOperations_DependsOnOperationId",
                table: "SandboxResourceOperations");

            migrationBuilder.DropColumn(
                name: "DependsOnOperationId",
                table: "SandboxResourceOperations");
        }
    }
}
