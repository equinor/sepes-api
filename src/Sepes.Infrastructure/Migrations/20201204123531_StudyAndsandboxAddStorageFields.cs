using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class StudyAndsandboxAddStorageFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudySpecificDatasetsResourceGroup",
                table: "Studies",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageAccountId",
                table: "Datasets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudySpecificDatasetsResourceGroup",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "StorageAccountId",
                table: "Datasets");
        }
    }
}
