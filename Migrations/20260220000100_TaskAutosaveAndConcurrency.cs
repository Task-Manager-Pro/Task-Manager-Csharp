using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Migrations
{
    public partial class TaskAutosaveAndConcurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstimateMinutes",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Tasks",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "SpentMinutes",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Todo");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FinishedAt",
                table: "Tasks",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "DueDate", table: "Tasks");
            migrationBuilder.DropColumn(name: "EstimateMinutes", table: "Tasks");
            migrationBuilder.DropColumn(name: "Order", table: "Tasks");
            migrationBuilder.DropColumn(name: "RowVersion", table: "Tasks");
            migrationBuilder.DropColumn(name: "SpentMinutes", table: "Tasks");
            migrationBuilder.DropColumn(name: "State", table: "Tasks");
            migrationBuilder.DropColumn(name: "TenantId", table: "Tasks");
            migrationBuilder.DropColumn(name: "UpdatedAt", table: "Tasks");
            migrationBuilder.DropColumn(name: "UpdatedBy", table: "Tasks");
            migrationBuilder.DropColumn(name: "TenantId", table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FinishedAt",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
