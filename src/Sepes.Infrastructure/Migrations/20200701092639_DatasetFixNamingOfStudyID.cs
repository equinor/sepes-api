using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class DatasetFixNamingOfStudyID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudyID",
                table: "Datasets");

            migrationBuilder.AddColumn<int>(
                name: "StudyNo",
                table: "Datasets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudyNo",
                table: "Datasets");

            migrationBuilder.AddColumn<int>(
                name: "StudyID",
                table: "Datasets",
                type: "int",
                nullable: true);
        }
    }
}
