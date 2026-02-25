using System.ComponentModel.DataAnnotations;

namespace MVC_Movie.Models
{
    public class PurchaseNotification
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [MaxLength(250)]
        public string Message { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
