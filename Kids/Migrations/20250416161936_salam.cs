using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kids.Migrations
{
    /// <inheritdoc />
    public partial class salam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descreption",
                table: "CourseGroups",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descreption",
                table: "CourseGroups");
        }
    }
}
