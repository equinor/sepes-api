using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class addmoreregions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Regions",
                nullable: true);

            migrationBuilder.Sql("UPDATE [dbo].[Regions] SET Disabled = 0, [Order] = 0 WHERE [Key] = 'norwayeast'");
            migrationBuilder.Sql("UPDATE [dbo].[Regions] SET Disabled = 0, [Order] = 1 WHERE [Key] = 'northeurope'");
            migrationBuilder.Sql("UPDATE [dbo].[Regions] SET Disabled = 0, [Order] = 2 WHERE [Key] = 'westeurope'");
            migrationBuilder.Sql("INSERT INTO [dbo].[Regions] ([Key], Created, CreatedBy, [Name], Disabled, [Order]) VALUES ('norwaywest', getutcdate(), 'migration', 'Norway West', 0, 3)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Regions");
         
            migrationBuilder.Sql("UPDATE [dbo].[Regions] SET Disabled = 1 WHERE [Key] = 'northeurope'");
            migrationBuilder.Sql("UPDATE [dbo].[Regions] SET Disabled = 1 WHERE [Key] = 'westeurope'");
            migrationBuilder.Sql("UPDATE [dbo].[Regions] SET Disabled = 1 WHERE [Key] = 'norwaywest'"); 
        }
    }
}
