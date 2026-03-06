namespace MVC_Movie.Models
{
    public class AdminDashboardVM
    {
        public int TotalUsers { get; set; }
        public int TotalMovies { get; set; }
        public int ActiveRentals { get; set; }
        public int ReturnedRentals { get; set; }
        public int ExpiredRentals { get; set; }
        public int TotalPurchases { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalDiscount { get; set; }
    }

    public class UserWithRoleVM
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

}
