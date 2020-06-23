using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class StudyDataset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SandBoxes_Studies_StudyId",
                table: "SandBoxes");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyDataset_DataSets_DatasetId",
                table: "StudyDataset");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyDataset_Studies_StudyId",
                table: "StudyDataset");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SandBoxes",
                table: "SandBoxes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DataSets",
                table: "DataSets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyDataset",
                table: "StudyDataset");

            migrationBuilder.RenameTable(
                name: "SandBoxes",
                newName: "Sandboxes");

            migrationBuilder.RenameTable(
                name: "DataSets",
                newName: "Datasets");

            migrationBuilder.RenameTable(
                name: "StudyDataset",
                newName: "StudyDatasets");

            migrationBuilder.RenameIndex(
                name: "IX_SandBoxes_StudyId",
                table: "Sandboxes",
                newName: "IX_Sandboxes_StudyId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyDataset_DatasetId",
                table: "StudyDatasets",
                newName: "IX_StudyDatasets_DatasetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sandboxes",
                table: "Sandboxes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Datasets",
                table: "Datasets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyDatasets",
                table: "StudyDatasets",
                columns: new[] { "StudyId", "DatasetId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Sandboxes_Studies_StudyId",
                table: "Sandboxes",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyDatasets_Datasets_DatasetId",
                table: "StudyDatasets",
                column: "DatasetId",
                principalTable: "Datasets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyDatasets_Studies_StudyId",
                table: "StudyDatasets",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sandboxes_Studies_StudyId",
                table: "Sandboxes");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyDatasets_Datasets_DatasetId",
                table: "StudyDatasets");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyDatasets_Studies_StudyId",
                table: "StudyDatasets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sandboxes",
                table: "Sandboxes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Datasets",
                table: "Datasets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyDatasets",
                table: "StudyDatasets");

            migrationBuilder.RenameTable(
                name: "Sandboxes",
                newName: "SandBoxes");

            migrationBuilder.RenameTable(
                name: "Datasets",
                newName: "DataSets");

            migrationBuilder.RenameTable(
                name: "StudyDatasets",
                newName: "StudyDataset");

            migrationBuilder.RenameIndex(
                name: "IX_Sandboxes_StudyId",
                table: "SandBoxes",
                newName: "IX_SandBoxes_StudyId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyDatasets_DatasetId",
                table: "StudyDataset",
                newName: "IX_StudyDataset_DatasetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SandBoxes",
                table: "SandBoxes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataSets",
                table: "DataSets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyDataset",
                table: "StudyDataset",
                columns: new[] { "StudyId", "DatasetId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SandBoxes_Studies_StudyId",
                table: "SandBoxes",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyDataset_DataSets_DatasetId",
                table: "StudyDataset",
                column: "DatasetId",
                principalTable: "DataSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyDataset_Studies_StudyId",
                table: "StudyDataset",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
