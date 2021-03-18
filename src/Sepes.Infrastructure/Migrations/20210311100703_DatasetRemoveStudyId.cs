using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class DatasetRemoveStudyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE dbo.Datasets SET StudySpecific = 1 WHERE ISNULL(StudyId, 0) > 0");

            migrationBuilder.DropColumn(
                name: "StudyId",
                table: "Datasets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudyId",
                table: "Datasets",
                type: "int",
                nullable: true);
        }
    }
}
