using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class StudyAddWbsValidation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WbsCodeValid",
                table: "Studies",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WbsCodeValidatedAt",
                table: "Studies",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WbsCodeCache",
                columns: table => new
                {
                    WbsCode = table.Column<string>(nullable: false),
                    Expires = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WbsCodeCache", x => x.WbsCode);
                });

            migrationBuilder.Sql("UPDATE dbo.Studies SET WbsCodeValid = ISNULL(WbsCodeValid, 0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WbsCodeCache");

            migrationBuilder.DropColumn(
                name: "WbsCodeValid",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "WbsCodeValidatedAt",
                table: "Studies");
        }
    }
}
