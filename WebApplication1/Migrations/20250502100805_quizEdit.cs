using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class quizEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Units_UnitId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_UnitId",
                table: "Quizzes");

            migrationBuilder.AddColumn<string>(
                name: "Quiz",
                table: "Units",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_UnitId",
                table: "Quizzes",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Units_UnitId",
                table: "Quizzes",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Units_UnitId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_UnitId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Quiz",
                table: "Units");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_UnitId",
                table: "Quizzes",
                column: "UnitId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Units_UnitId",
                table: "Quizzes",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
