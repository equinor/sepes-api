using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class RegionsAddRelevantEntries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM [dbo].[Regions] WHERE [Key] = 'norwayeast') BEGIN INSERT INTO [dbo].[Regions] VALUES ('norwayeast', getutcdate(), 'migration', 'Norway East', 0) END");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM [dbo].[Regions] WHERE [Key] = 'northeurope') BEGIN INSERT INTO [dbo].[Regions] VALUES ('northeurope', getutcdate(), 'migration', 'North Europe', 1) END");
            migrationBuilder.Sql("IF NOT EXISTS (SELECT * FROM [dbo].[Regions] WHERE [Key] = 'westeurope') BEGIN INSERT INTO [dbo].[Regions] VALUES ('westeurope', getutcdate(), 'migration', 'West Europe', 1) END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[Regions] WHERE [Key] IN ('norwayeast', 'northeurope', 'westeurope')");
        }
    }
}
