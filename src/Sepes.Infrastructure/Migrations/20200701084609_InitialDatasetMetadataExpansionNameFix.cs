using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class InitialDatasetMetadataExpansionNameFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Datasets",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(26)",
                oldMaxLength: 26);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Datasets",
                type: "nvarchar(26)",
                maxLength: 26,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 64);
        }
    }
}
