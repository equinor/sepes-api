using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class RenameDatasetStudyNoToStudyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudyNo",
                table: "Datasets");

            migrationBuilder.AddColumn<int>(
                name: "StudyId",
                table: "Datasets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudyId",
                table: "Datasets");

            migrationBuilder.AddColumn<int>(
                name: "StudyNo",
                table: "Datasets",
                type: "int",
                nullable: true);
        }
    }
}
