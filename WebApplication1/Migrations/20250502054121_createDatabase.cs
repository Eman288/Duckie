using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class createDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Units_UnitId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "level",
                table: "Units");

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Units",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PlacementQuizzes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacementQuizzes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Units_AdminId",
                table: "Units",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_AdminId",
                table: "Lessons",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Admins_AdminId",
                table: "Lessons",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Units_UnitId",
                table: "Quizzes",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Admins_AdminId",
                table: "Units",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Admins_AdminId",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Units_UnitId",
                table: "Quizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_Admins_AdminId",
                table: "Units");

            migrationBuilder.DropTable(
                name: "PlacementQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_Units_AdminId",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_AdminId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Lessons");

            migrationBuilder.AddColumn<string>(
                name: "level",
                table: "Units",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Units_UnitId",
                table: "Quizzes",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
