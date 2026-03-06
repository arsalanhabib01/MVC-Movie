namespace MVC_Movie.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Code { get; set; }   // e.g. SAVE10
        public decimal DiscountPercentage { get; set; }  // e.g. 10 = 10%
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int UsageLimit { get; set; } // e.g. 10 uses total
        public int TimesUsed { get; set; } // e.g. 5 times used so far
        public CouponType CouponType { get; set; }
    }
}
