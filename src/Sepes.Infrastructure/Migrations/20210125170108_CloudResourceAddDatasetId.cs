using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class CloudResourceAddDatasetId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.datasets WHERE StudyId IS NOT NULL AND StudyId NOT IN (SELECT ID FROM dbo.Studies)");

            migrationBuilder.AddColumn<int>(
                name: "DatasetId",
                table: "CloudResources",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Datasets_StudyId",
                table: "Datasets",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_CloudResources_DatasetId",
                table: "CloudResources",
                column: "DatasetId");

            migrationBuilder.AddForeignKey(
                name: "FK_CloudResources_Datasets_DatasetId",
                table: "CloudResources",
                column: "DatasetId",
                principalTable: "Datasets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Datasets_Studies_StudyId",
                table: "Datasets",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CloudResources_Datasets_DatasetId",
                table: "CloudResources");

            migrationBuilder.DropForeignKey(
                name: "FK_Datasets_Studies_StudyId",
                table: "Datasets");

            migrationBuilder.DropIndex(
                name: "IX_Datasets_StudyId",
                table: "Datasets");

            migrationBuilder.DropIndex(
                name: "IX_CloudResources_DatasetId",
                table: "CloudResources");

            migrationBuilder.DropColumn(
                name: "DatasetId",
                table: "CloudResources");
        }
    }
}
