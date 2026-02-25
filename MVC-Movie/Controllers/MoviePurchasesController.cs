using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Movie.Data;
using MVC_Movie.Models;
using System.Security.Claims;

namespace MVC_Movie.Controllers
{
    [Authorize]
    public class MoviePurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviePurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> BuyMovie(int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var movie = await _context.Movie.FindAsync(movieId);

            if (movie == null)
                return NotFound("Movie not found");

            bool alreadyPurchased = await _context.MoviePurchases
                .AnyAsync(p => p.MovieId == movieId && p.UserId == userId);

            if (alreadyPurchased)
                return BadRequest("Already purchased");

            // 🔔 ADD NOTIFICATION HERE
            var notification = new PurchaseNotification
            {
                UserId = userId,
                Message = $"You successfully purchased '{movie.Title}' 🎬",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.PurchaseNotifications.Add(notification);
    
            var purchase = new MoviePurchase
            {
                MovieId = movieId,
                UserId = userId,
                PurchaseDate = DateTime.UtcNow,
                Status = PurchaseStatus.Completed,
                PurchasePrice = 25
            };

            //movie.IsAvailable = false;

            _context.MoviePurchases.Add(purchase);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyPurchases", "MoviePurchases");
        }

        public async Task<IActionResult> MyPurchases()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var purchases = await _context.MoviePurchases
                .Include(r => r.Movie)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.PurchaseDate)
                .ToListAsync();

            return View(purchases);
        }
    }
}
