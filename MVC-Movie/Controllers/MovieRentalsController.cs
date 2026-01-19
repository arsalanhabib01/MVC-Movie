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

            var rental = new MovieRental
            {
                MovieId = movieId,
                UserId = userId,
                RentedAt = DateTime.UtcNow,
                DueAt = DateTime.UtcNow.AddDays(3),
                Status = RentalStatus.Active
            };

            movie.IsAvailable = false;

            _context.MovieRentals.Add(rental);
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
