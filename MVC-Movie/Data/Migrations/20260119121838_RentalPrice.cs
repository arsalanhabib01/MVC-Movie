using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Movie.Data.Migrations
{
    /// <inheritdoc />
    public partial class RentalPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RentalPrice",
                table: "MovieRentals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentalPrice",
                table: "MovieRentals");
        }
    }
}
