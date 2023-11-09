using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Migrations
{
    /// <inheritdoc />
    public partial class RenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_CategorieTask_CategorieTaskId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorieTask",
                table: "CategorieTask");

            migrationBuilder.RenameTable(
                name: "CategorieTask",
                newName: "CategorieTasks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorieTasks",
                table: "CategorieTasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_CategorieTasks_CategorieTaskId",
                table: "Tasks",
                column: "CategorieTaskId",
                principalTable: "CategorieTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_CategorieTasks_CategorieTaskId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorieTasks",
                table: "CategorieTasks");

            migrationBuilder.RenameTable(
                name: "CategorieTasks",
                newName: "CategorieTask");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorieTask",
                table: "CategorieTask",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_CategorieTask_CategorieTaskId",
                table: "Tasks",
                column: "CategorieTaskId",
                principalTable: "CategorieTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
