using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class VmImageSearchPropertiesContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"SET IDENTITY_INSERT [dbo].[VmImageSearchProperties] ON 
GO
INSERT[dbo].[VmImageSearchProperties]([Id], [Created], [CreatedBy], [Publisher], [Offer], [Sku], [DisplayValue], [Category], [PartOfRecommended]) VALUES(1, GETUTCDATE(), N'krst@equinor.com', N'MicrosoftWindowsServer', N'WindowsServer', N'2019-Datacenter-Core', N'Windows Server Datacenter Core 2019', N'windows', 1)
GO
INSERT[dbo].[VmImageSearchProperties] ([Id], [Created], [CreatedBy], [Publisher], [Offer], [Sku], [DisplayValue], [Category], [PartOfRecommended]) VALUES(2, GETUTCDATE(), N'krst@equinor.com', N'RedHat', N'RHEL', N'8_4', N'RedHat Enterprise', N'linux', 1)
GO
INSERT[dbo].[VmImageSearchProperties] ([Id], [Created], [CreatedBy], [Publisher], [Offer], [Sku], [DisplayValue], [Category], [PartOfRecommended]) VALUES(3, GETUTCDATE(), N'krst@equinor.com', N'MicrosoftWindowsServer', N'WindowsServer', N'2019-Datacenter', N'Windows Server Datacenter 2019', N'windows', 1)
GO
INSERT[dbo].[VmImageSearchProperties] ([Id], [Created], [CreatedBy], [Publisher], [Offer], [Sku], [DisplayValue], [Category], [PartOfRecommended]) VALUES(4, GETUTCDATE(), N'krst@equinor.com', N'MicrosoftWindowsServer', N'WindowsServer', N'2016-Datacenter', N'Windows Server Datacenter 2016', N'windows', 0)
GO
INSERT[dbo].[VmImageSearchProperties] ([Id], [Created], [CreatedBy], [Publisher], [Offer], [Sku], [DisplayValue], [Category], [PartOfRecommended]) VALUES(5, GETUTCDATE(), N'krst@equinor.com', N'Debian', N'debian-10', N'10', N'Debian 10', N'linux', 1)
GO
INSERT[dbo].[VmImageSearchProperties] ([Id], [Created], [CreatedBy], [Publisher], [Offer], [Sku], [DisplayValue], [Category], [PartOfRecommended]) VALUES(6, GETUTCDATE(), N'krst@equinor.com', N'OpenLogic', N'CentOS', N'8_4', N'CentOS', N'linux', 1)
GO
INSERT[dbo].[VmImageSearchProperties] ([Id], [Created], [CreatedBy], [Publisher], [Offer], [Sku], [DisplayValue], [Category], [PartOfRecommended]) VALUES(7, GETUTCDATE(), N'krst@equinor.com', N'Oracle', N'Oracle-Linux', N'ol83-lvm', N'Oracle Linux LVM', N'linux', 1)
GO
INSERT[dbo].[VmImageSearchProperties] ([Id], [Created], [CreatedBy], [Publisher], [Offer], [Sku], [DisplayValue], [Category], [PartOfRecommended]) VALUES(8, GETUTCDATE(), N'krst@equinor.com', N'Canonical', N'UbuntuServer', N'18.04-LTS', N'Ubuntu Server Enterprise 18 LTS', N'linux', 1)
GO
SET IDENTITY_INSERT[dbo].[VmImageSearchProperties] OFF
GO");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
