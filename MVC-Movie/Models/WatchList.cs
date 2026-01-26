namespace MVC_Movie.Models
{
    public class WatchList
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public string UserId { get; set; }
        public DateTime AddedAt { get; set; }

    }
}
