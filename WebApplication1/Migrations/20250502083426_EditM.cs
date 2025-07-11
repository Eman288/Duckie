using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class EditM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "level",
                table: "Units",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Situations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Conversations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Situations_AdminId",
                table: "Situations",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_AdminId",
                table: "Conversations",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Admins_AdminId",
                table: "Conversations",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Situations_Admins_AdminId",
                table: "Situations",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Admins_AdminId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Situations_Admins_AdminId",
                table: "Situations");

            migrationBuilder.DropIndex(
                name: "IX_Situations_AdminId",
                table: "Situations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_AdminId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "level",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Situations");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Conversations");
        }
    }
}
