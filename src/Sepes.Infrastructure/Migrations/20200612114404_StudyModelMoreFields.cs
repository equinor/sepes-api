using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class StudyModelMoreFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSets_Studies_StudyId",
                table: "DataSets");

            migrationBuilder.DropIndex(
                name: "IX_DataSets_StudyId",
                table: "DataSets");

            migrationBuilder.DropColumn(
                name: "StudyId",
                table: "DataSets");

            migrationBuilder.AddColumn<int>(
                name: "LogoId",
                table: "Studies",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Restricted",
                table: "Studies",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "StudyDataset",
                columns: table => new
                {
                    StudyId = table.Column<int>(nullable: false),
                    DatasetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyDataset", x => new { x.StudyId, x.DatasetId });
                    table.ForeignKey(
                        name: "FK_StudyDataset_DataSets_DatasetId",
                        column: x => x.DatasetId,
                        principalTable: "DataSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyDataset_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyLogo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Content = table.Column<byte[]>(nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_StudyDataset_DatasetId",
                table: "StudyDataset",
                column: "DatasetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Studies_StudyLogo_LogoId",
                table: "Studies",
                column: "LogoId",
                principalTable: "StudyLogo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Studies_StudyLogo_LogoId",
                table: "Studies");

            migrationBuilder.DropTable(
                name: "StudyDataset");

            migrationBuilder.DropTable(
                name: "StudyLogo");

            migrationBuilder.DropIndex(
                name: "IX_Studies_LogoId",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "LogoId",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "Restricted",
                table: "Studies");

            migrationBuilder.AddColumn<int>(
                name: "StudyId",
                table: "DataSets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DataSets_StudyId",
                table: "DataSets",
                column: "StudyId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataSets_Studies_StudyId",
                table: "DataSets",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
