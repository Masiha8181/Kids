using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kids.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageAddress",
                table: "CourseGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageTite",
                table: "CourseGalleries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageAddress",
                table: "ArticleGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageAddress",
                table: "AgeGroups",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageAddress",
                table: "CourseGroups");

            migrationBuilder.DropColumn(
                name: "ImageTite",
                table: "CourseGalleries");

            migrationBuilder.DropColumn(
                name: "ImageAddress",
                table: "ArticleGroups");

            migrationBuilder.DropColumn(
                name: "ImageAddress",
                table: "AgeGroups");
        }
    }
}
