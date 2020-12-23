using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class CloudResourceOperationQueueInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CloudResourceOperations",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QueueMessageId",
                table: "CloudResourceOperations",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QueueMessagePopReceipt",
                table: "CloudResourceOperations",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "QueueMessageVisibleAgainAt",
                table: "CloudResourceOperations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QueueMessageId",
                table: "CloudResourceOperations");

            migrationBuilder.DropColumn(
                name: "QueueMessagePopReceipt",
                table: "CloudResourceOperations");

            migrationBuilder.DropColumn(
                name: "QueueMessageVisibleAgainAt",
                table: "CloudResourceOperations");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CloudResourceOperations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
