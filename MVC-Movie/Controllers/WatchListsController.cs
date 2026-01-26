using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Movie.Data;
using MVC_Movie.Models;
using System.Security.Claims;


namespace MVC_Movie.Controllers
{
    [Authorize]
    public class WatchListsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WatchListsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var movie = await _context.Movie.FindAsync(movieId);

            if (movie == null)
                return NotFound("Movie not found");

            bool watchListExists = await _context.WatchLists
                .AnyAsync(w => w.UserId == userId && w.MovieId == movieId);

            if (!watchListExists)
            {
                var watchList = new WatchList
                {
                    UserId = userId,
                    MovieId = movieId,
                    AddedAt = DateTime.UtcNow,
                };
                _context.WatchLists.Add(watchList);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyWatchLists", "WatchLists");
        }

        // POST: Watchlist/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the watchlist entry for this user and movie
            var item = await _context.WatchLists
                .FirstOrDefaultAsync(w => w.Id == Id && w.UserId == userId);

            if (item == null)
                return NotFound("Movie not found in your watchlist");

            _context.WatchLists.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyWatchLists));
        }

        public async Task<IActionResult> MyWatchLists()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var watchLists = await _context.WatchLists
                .Include(r => r.Movie)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.AddedAt)
                .ToListAsync();

            return View(watchLists);
        }
    }
}
