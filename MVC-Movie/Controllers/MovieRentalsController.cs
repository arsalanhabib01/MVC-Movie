using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Movie.Data;
using MVC_Movie.Models;
using System.Security.Claims;

namespace MVC_Movie.Controllers
{
    [Authorize]
    public class MovieRentalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieRentalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> RentMovie(int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var movie = await _context.Movie.FindAsync(movieId);

            if (movie == null)
                return NotFound("Movie not found");

            // Create a random number generator
            //var random = new Random();

            var rental = new MovieRental
            {
                MovieId = movieId,
                UserId = userId,
                RentedAt = DateTime.UtcNow,
                DueAt = DateTime.UtcNow.AddDays(3),
                Status = RentalStatus.Active,
                RentalPrice = 7
                //RentalPrice = random.Next(5, 15)
            };

            movie.IsAvailable = false;

            _context.MovieRentals.Add(rental);

            // 🔔 ADD NOTIFICATION HERE
            var notification = new Notification
            {
                UserId = userId,
                Message = $"You successfully rented '{movie.Title}' 🎬. \n" +
                          $"Please return it by {rental.DueAt.ToLocalTime():MMM dd, yyyy hh:mm tt}.",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return RedirectToAction("MyRentals", "MovieRentals");
        }

        [HttpPost]
        public async Task<IActionResult> ReturnMovie(int rentalId)
        {
            var rental = await _context.MovieRentals
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r => r.Id == rentalId);

            if (rental == null)
                return NotFound("Rental not found");

            if (rental.Status != RentalStatus.Active)
                return BadRequest("Rental already closed");

            rental.Status = RentalStatus.Returned;
            rental.ReturnedAt = DateTime.UtcNow;
            rental.Movie.IsAvailable = true;

            // ✅ ADD Returned NOTIFICATION HERE
            var notification = new Notification
            {
                UserId = rental.UserId,
                Message = $"Return Confirmed ✅ '{rental.Movie.Title}' has been returned successfully.\n" +
                          $"We hope you enjoyed watching it! 🍿",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyRentals));
        }

        public async Task<IActionResult> MyRentals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var rentals = await _context.MovieRentals
                .Include(r => r.Movie)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RentedAt)
                .ToListAsync();

            return View(rentals);
        }
    }
}
