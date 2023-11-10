using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Migrations
{
    /// <inheritdoc />
    public partial class IncludeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_CategorieTask_CategorieTaskId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "CategorieTask");

            migrationBuilder.CreateTable(
                name: "CategorieTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorieTasks", x => x.Id);
                });

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

            migrationBuilder.DropTable(
                name: "CategorieTasks");

            migrationBuilder.CreateTable(
                name: "CategorieTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorieTask", x => x.Id);
                });

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
