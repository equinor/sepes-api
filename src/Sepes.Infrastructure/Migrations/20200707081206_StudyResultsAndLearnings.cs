using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class StudyResultsAndLearnings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResultsAndLearnings",
                table: "Studies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceGroupId",
                table: "AzureResources",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultsAndLearnings",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "ResourceGroupId",
                table: "AzureResources");
        }
    }
}
