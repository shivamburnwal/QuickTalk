using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickTalk.Api.Data.Migrations
{
    public partial class SeedAdminData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "AvatarUrl", "CreatedAt", "DisplayName", "Email", "LastLogin", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, null, new DateTime(2025, 2, 2, 8, 19, 4, 381, DateTimeKind.Utc).AddTicks(1122), null, "admin@quicktalk.com", null, "e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7", "Admin", "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");
        }
    }
}
