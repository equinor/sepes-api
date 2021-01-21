using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class CloudResourceRenamedDeletedToDeletedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {  
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CloudResources",
                nullable: true);

            migrationBuilder.Sql("UPDATE dbo.CloudResources SET DeletedAt = Deleted");

            migrationBuilder.DropColumn(
               name: "Deleted",
               table: "CloudResources");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
            name: "Deleted",
            table: "CloudResources",
            type: "datetime2",
            nullable: true);

            migrationBuilder.Sql("UPDATE dbo.CloudResources SET Deleted = DeletedAt");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CloudResources");

        
        }
    }
}
