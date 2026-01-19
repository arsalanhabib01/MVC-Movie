
namespace MVC_Movie.Models
{
    public class MovieRental
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        // Foreign Key 
        public string UserId { get; set; } 

        public DateTime RentedAt { get; set; }
        public DateTime DueAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        
        public int RentalPrice { get; set; }

        public RentalStatus Status { get; set; }
    }
}
