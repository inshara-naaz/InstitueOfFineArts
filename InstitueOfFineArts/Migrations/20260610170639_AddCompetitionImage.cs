using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitueOfFineArts.Migrations
{
    /// <inheritdoc />
    public partial class AddCompetitionImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Competitions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Competitions");
        }
    }
}
