using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class ConnectedStudiesWithSandboxAndDataset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudyId",
                table: "SandBoxes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudyId",
                table: "DataSets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SandBoxes_StudyId",
                table: "SandBoxes",
                column: "StudyId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_SandBoxes_Studies_StudyId",
                table: "SandBoxes",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSets_Studies_StudyId",
                table: "DataSets");

            migrationBuilder.DropForeignKey(
                name: "FK_SandBoxes_Studies_StudyId",
                table: "SandBoxes");

            migrationBuilder.DropIndex(
                name: "IX_SandBoxes_StudyId",
                table: "SandBoxes");

            migrationBuilder.DropIndex(
                name: "IX_DataSets_StudyId",
                table: "DataSets");

            migrationBuilder.DropColumn(
                name: "StudyId",
                table: "SandBoxes");

            migrationBuilder.DropColumn(
                name: "StudyId",
                table: "DataSets");
        }
    }
}
