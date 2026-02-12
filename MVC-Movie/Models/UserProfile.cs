using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Movie.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        // Foreign key to IdentityUser
        public string UserId { get; set; }

        // Navigation property
        public IdentityUser User { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ProfileImage { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
