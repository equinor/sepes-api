using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class StudyDeleteDatasetField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudySpecificDatasetsResourceGroup",
                table: "Studies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudySpecificDatasetsResourceGroup",
                table: "Studies",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);
        }
    }
}
