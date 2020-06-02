using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class ReDefiningStudyAwayFromJson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonData",
                table: "Studies");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Studies",
                nullable: false,
                defaultValueSql: "getutcdate()");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Studies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Studies",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Studies",
                nullable: false,
                defaultValueSql: "getutcdate()");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Studies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WbsCode",
                table: "Studies",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DataSets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SandBoxes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(nullable: true),
                    Updated = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SandBoxes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataSets");

            migrationBuilder.DropTable(
                name: "SandBoxes");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "WbsCode",
                table: "Studies");

            migrationBuilder.AddColumn<string>(
                name: "JsonData",
                table: "Studies",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);
        }
    }
}
