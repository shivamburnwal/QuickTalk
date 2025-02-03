using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickTalk.Api.Data.Migrations
{
    public partial class AddAdminFeatureGroupChatrooms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "UserChatrooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 2, 3, 17, 31, 8, 737, DateTimeKind.Utc).AddTicks(1567));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "UserChatrooms");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 2, 2, 14, 30, 26, 450, DateTimeKind.Utc).AddTicks(7899));
        }
    }
}
