using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class updateUnit2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Pictures",
                table: "Units",
                newName: "Picture");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Situations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Situations");

            migrationBuilder.RenameColumn(
                name: "Picture",
                table: "Units",
                newName: "Pictures");
        }
    }
}
