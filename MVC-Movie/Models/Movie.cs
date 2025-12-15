using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Movie.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }

        [Range(0.0, 10.0)]
        [Column(TypeName = "decimal(3,1)")]   // allows values like 3.1
        public decimal Rating { get; set; }
        public string UserId { get; set; } // to link movie to a user
        public string? FileName { get; set; } // will be stored in DB

        [NotMapped]
        public IFormFile? FileForm { get; set; }

    }
}
