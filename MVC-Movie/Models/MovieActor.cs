using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Movie.Models
{
    public class MovieActor
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(100)]
        public string? Nationality { get; set; }

        public string Biography { get; set; }

        public string? ProfileImage { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
