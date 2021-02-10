using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class StudyMadeClosedNonNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [dbo].[Studies] SET Closed = ISNULL(Closed, 0)");

            migrationBuilder.AlterColumn<bool>(
                name: "Closed",
                table: "Studies",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Closed",
                table: "Studies",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool));
        }
    }
}
