using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Movie.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_PurchaseMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "MoviePurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountApplied",
                table: "MoviePurchases",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "MoviePurchases");

            migrationBuilder.DropColumn(
                name: "DiscountApplied",
                table: "MoviePurchases");
        }
    }
}
