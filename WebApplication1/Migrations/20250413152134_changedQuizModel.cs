using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class changedQuizModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Removed renaming, since we just want a new column
            migrationBuilder.AddColumn<int>(
                name: "Lesson",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Undo the change: remove the column
            migrationBuilder.DropColumn(
                name: "Lesson",
                table: "Quizzes");
        }
    }
}
