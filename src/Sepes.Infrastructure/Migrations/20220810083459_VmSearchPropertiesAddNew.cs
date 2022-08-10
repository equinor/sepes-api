using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sepes.Infrastructure.Migrations
{
    public partial class VmSearchPropertiesAddNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"SET IDENTITY_INSERT [dbo].[VmImageSearchProperties] ON 
GO
INSERT[dbo].[VmImageSearchProperties]([Id], [Created], [CreatedBy], [Publisher], [Offer], [Sku], [DisplayValue], [Category], [PartOfRecommended]) VALUES(11, GETUTCDATE(), N'bonm@equinor.com', N'MicrosoftWindowsServer', N'WindowsServer', N'2022-Datacenter-Core', N'Windows Server Datacenter Core 2022', N'windows', 1)
INSERT[dbo].[VmImageSearchProperties]([Id], [Created], [CreatedBy], [Publisher], [Offer], [Sku], [DisplayValue], [Category], [PartOfRecommended]) VALUES(12, GETUTCDATE(), N'bonm@equinor.com', N'MicrosoftWindowsServer', N'WindowsServer', N'2022-Datacenter', N'Windows Server Datacenter 2022', N'windows', 1)
GO
SET IDENTITY_INSERT[dbo].[VmImageSearchProperties] OFF
GO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
