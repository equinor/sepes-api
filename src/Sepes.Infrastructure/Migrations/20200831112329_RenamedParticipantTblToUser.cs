using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sepes.Infrastructure.Migrations
{
    public partial class RenamedParticipantTblToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[StudyParticipants]");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyParticipants_Participants_ParticipantId",
                table: "StudyParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyParticipants_Studies_StudyId",
                table: "StudyParticipants");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyParticipants",
                table: "StudyParticipants");

            migrationBuilder.DropIndex(
                name: "IX_StudyParticipants_ParticipantId",
                table: "StudyParticipants");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "StudyParticipants");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "StudyParticipants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyParticipants",
                table: "StudyParticipants",
                columns: new[] { "StudyId", "UserId", "RoleName" });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    Updated = table.Column<DateTime>(nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedBy = table.Column<string>(maxLength: 64, nullable: true),
                    FullName = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    TenantId = table.Column<string>(nullable: true),
                    ObjectId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyParticipants_UserId",
                table: "StudyParticipants",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyParticipants_Studies_StudyId",
                table: "StudyParticipants",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyParticipants_Users_UserId",
                table: "StudyParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyParticipants_Studies_StudyId",
                table: "StudyParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyParticipants_Users_UserId",
                table: "StudyParticipants");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyParticipants",
                table: "StudyParticipants");

            migrationBuilder.DropIndex(
                name: "IX_StudyParticipants_UserId",
                table: "StudyParticipants");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StudyParticipants");

            migrationBuilder.AddColumn<int>(
                name: "ParticipantId",
                table: "StudyParticipants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyParticipants",
                table: "StudyParticipants",
                columns: new[] { "StudyId", "ParticipantId", "RoleName" });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyParticipants_ParticipantId",
                table: "StudyParticipants",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyParticipants_Participants_ParticipantId",
                table: "StudyParticipants",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyParticipants_Studies_StudyId",
                table: "StudyParticipants",
                column: "StudyId",
                principalTable: "Studies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
