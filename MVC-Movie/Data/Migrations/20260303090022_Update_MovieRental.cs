using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Movie.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_MovieRental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "MovieRentals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountApplied",
                table: "MovieRentals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "MovieRentals");

            migrationBuilder.DropColumn(
                name: "DiscountApplied",
                table: "MovieRentals");
        }
    }
}
