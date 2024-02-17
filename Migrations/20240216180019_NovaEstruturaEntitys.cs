using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Migrations
{
    /// <inheritdoc />
    public partial class NovaEstruturaEntitys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isLogged",
                table: "Users",
                newName: "IsLogged");

            migrationBuilder.RenameColumn(
                name: "LoginTime",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                table: "Users",
                type: "varbinary(max)",
                nullable: true,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedAt",
                table: "Tasks",
                type: "datetime2",
                nullable: true,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FinishedAt",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "IsLogged",
                table: "Users",
                newName: "isLogged");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "LoginTime");
        }
    }
}
