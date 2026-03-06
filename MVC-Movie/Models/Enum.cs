namespace MVC_Movie.Models
{
    public enum RentalStatus
    {
        Active,
        Returned,
        Expired
    }

    public enum PurchaseStatus
    {
        Completed,
        Refunded,
        Cancelled
    }

    public enum CouponType
    {
        Rent,
        Purchase,
        Birthday
    }

}
