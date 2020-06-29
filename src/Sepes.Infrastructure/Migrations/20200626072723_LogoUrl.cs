using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class LogoUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Studies_StudyLogo_LogoId",
                table: "Studies");

            migrationBuilder.DropTable(
                name: "StudyLogo");

            migrationBuilder.DropIndex(
                name: "IX_Studies_LogoId",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "LogoId",
                table: "Studies");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Studies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Studies");

            migrationBuilder.AddColumn<int>(
                name: "LogoId",
                table: "Studies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudyLogo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyLogo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Studies_LogoId",
                table: "Studies",
                column: "LogoId",
                unique: true,
                filter: "[LogoId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Studies_StudyLogo_LogoId",
                table: "Studies",
                column: "LogoId",
                principalTable: "StudyLogo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
