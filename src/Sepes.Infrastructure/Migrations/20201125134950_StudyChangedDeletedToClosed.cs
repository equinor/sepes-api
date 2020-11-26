using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class StudyChangedDeletedToClosed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Closed",
                table: "Studies",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "Studies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Studies",
                maxLength: 64,
                nullable: true);

            migrationBuilder.Sql("UPDATE [dbo].[Studies] SET [Closed] = [Deleted], [ClosedAt] = [DeletedAt], [ClosedBy] = [DeletedBy]");

            migrationBuilder.DropColumn(
            name: "Deleted",
            table: "Studies");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Studies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
             name: "Deleted",
             table: "Studies",
             type: "bit",
             nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Studies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Studies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql("UPDATE [dbo].[Studies] SET [Deleted] = [Closed], [DeletedAt] = [ClosedAt], [DeletedBy] = [ClosedBy]");

            migrationBuilder.DropColumn(
                name: "Closed",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Studies");

          

         
        }
    }
}
