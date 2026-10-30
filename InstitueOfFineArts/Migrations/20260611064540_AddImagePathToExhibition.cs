using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitueOfFineArts.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToExhibition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Exhibitions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Exhibitions");
        }
    }
}
