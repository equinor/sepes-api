using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class InitialDatasetMetadataExpansion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Datasets",
                maxLength: 26,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AreaL1",
                table: "Datasets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AreaL2",
                table: "Datasets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Asset",
                table: "Datasets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BADataOwner",
                table: "Datasets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Classification",
                table: "Datasets",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryOfOrigin",
                table: "Datasets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DataId",
                table: "Datasets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Datasets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LRAId",
                table: "Datasets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Datasets",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceSystem",
                table: "Datasets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Datasets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaL1",
                table: "Datasets");

            migrationBuilder.DropColumn(
                name: "AreaL2",
                table: "Datasets");

            migrationBuilder.DropColumn(
                name: "Asset",
                table: "Datasets");

            migrationBuilder.DropColumn(
                name: "BADataOwner",
                table: "Datasets");

            migrationBuilder.DropColumn(
                name: "Classification",
                table: "Datasets");

            migrationBuilder.DropColumn(
                name: "CountryOfOrigin",
                table: "Datasets");

            migrationBuilder.DropColumn(
                name: "DataId",
                table: "Datasets");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Datasets");

            migrationBuilder.DropColumn(
                name: "LRAId",
                table: "Datasets");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Datasets");

            migrationBuilder.DropColumn(
                name: "SourceSystem",
                table: "Datasets");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Datasets");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Datasets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 26);
        }
    }
}
