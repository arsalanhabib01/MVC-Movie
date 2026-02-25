using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Movie.Data;
using System.Security.Claims;

namespace MVC_Movie.Controllers
{
    public class PurchaseNotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseNotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Notifications
        public async Task<IActionResult> MyNotification()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var purchaseNotifications = await _context.PurchaseNotifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(purchaseNotifications);
        }

        // Mark as Read
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.PurchaseNotifications.FindAsync(id);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MyNotification));
        }
    }
}
