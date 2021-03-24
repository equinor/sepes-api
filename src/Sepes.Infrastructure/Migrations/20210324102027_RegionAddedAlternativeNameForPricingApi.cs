using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class RegionAddedAlternativeNameForPricingApi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeyInPriceApi",
                table: "Regions",
                maxLength: 32,
                nullable: true);

            migrationBuilder.Sql("UPDATE [dbo].[Regions] SET [KeyInPriceApi] = 'norwayeast' WHERE [Key] = 'norwayeast'");
            migrationBuilder.Sql("UPDATE [dbo].[Regions] SET [KeyInPriceApi] = 'norwaywest' WHERE [Key] = 'norwaywest'");
            migrationBuilder.Sql("UPDATE [dbo].[Regions] SET [KeyInPriceApi] = 'europenorth' WHERE [Key] = 'northeurope'");
            migrationBuilder.Sql("UPDATE [dbo].[Regions] SET [KeyInPriceApi] = 'europewest' WHERE [Key] = 'westeurope'");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyInPriceApi",
                table: "Regions");
        }
    }
}
