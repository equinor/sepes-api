using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class StudyAndDatasetRemovedFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Datasets_Studies_StudyId",
                table: "Datasets");

            migrationBuilder.DropIndex(
                name: "IX_Datasets_StudyId",
                table: "Datasets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Datasets_StudyId",
                table: "Datasets",
                column: "StudyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Datasets_Studies_StudyId",
                table: "Datasets",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
