using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class ResourceOperationAddQUeueMessageDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QueueMessageId",
                table: "SandboxResourceOperations",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QueueMessagePopReceipt",
                table: "SandboxResourceOperations",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "QueueMessageVisibleAgainAt",
                table: "SandboxResourceOperations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QueueMessageId",
                table: "SandboxResourceOperations");

            migrationBuilder.DropColumn(
                name: "QueueMessagePopReceipt",
                table: "SandboxResourceOperations");

            migrationBuilder.DropColumn(
                name: "QueueMessageVisibleAgainAt",
                table: "SandboxResourceOperations");
        }
    }
}
