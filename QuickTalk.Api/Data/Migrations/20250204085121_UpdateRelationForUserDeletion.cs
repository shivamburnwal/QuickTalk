using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickTalk.Api.Data.Migrations
{
    public partial class UpdateRelationForUserDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageReactions_Users_UserID",
                table: "MessageReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_UserID",
                table: "Messages");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Messages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "MessageReactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 2, 4, 8, 51, 20, 434, DateTimeKind.Utc).AddTicks(8082));

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReactions_Users_UserID",
                table: "MessageReactions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_UserID",
                table: "Messages",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageReactions_Users_UserID",
                table: "MessageReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_UserID",
                table: "Messages");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "MessageReactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 2, 3, 17, 31, 8, 737, DateTimeKind.Utc).AddTicks(1567));

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReactions_Users_UserID",
                table: "MessageReactions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_UserID",
                table: "Messages",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
