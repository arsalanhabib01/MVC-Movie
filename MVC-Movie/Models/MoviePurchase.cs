namespace MVC_Movie.Models
{
    public class MoviePurchase
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public string UserId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int PurchasePrice { get; set; }
        public PurchaseStatus Status { get; set; }
        public decimal DiscountApplied { get; set; }
        public string? CouponCode { get; set; }
    }
}
