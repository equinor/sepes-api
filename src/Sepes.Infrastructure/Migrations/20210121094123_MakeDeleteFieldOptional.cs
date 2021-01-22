using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class MakeDeleteFieldOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE dbo.Sandboxes SET Deleted = 0 WHERE Deleted IS NULL");
            migrationBuilder.Sql("UPDATE dbo.Datasets SET Deleted = 0 WHERE Deleted IS NULL");
            migrationBuilder.Sql("UPDATE dbo.CloudResources SET Deleted = 0 WHERE Deleted IS NULL");

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "Sandboxes",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "Datasets",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "CloudResources",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "Sandboxes",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "Datasets",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "Deleted",
                table: "CloudResources",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));
        }
    }
}
