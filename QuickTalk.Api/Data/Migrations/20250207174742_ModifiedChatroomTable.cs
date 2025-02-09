using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickTalk.Api.Data.Migrations
{
    public partial class ModifiedChatroomTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Chatrooms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 2, 7, 17, 47, 41, 630, DateTimeKind.Utc).AddTicks(2580));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Chatrooms");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 2, 4, 8, 51, 20, 434, DateTimeKind.Utc).AddTicks(8082));
        }
    }
}
