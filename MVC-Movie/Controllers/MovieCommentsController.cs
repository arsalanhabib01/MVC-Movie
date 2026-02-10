using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Movie.Data;
using MVC_Movie.Models;
using System.Security.Claims;

namespace MVC_Movie.Controllers
{
    [Authorize]
    public class MovieCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieCommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int movieId, string commentText)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var movie = await _context.Movie.FindAsync(movieId);
            if (movie == null)
                return NotFound("Movie not found");

            var comment = new MovieComment
            {
                UserId = userId,
                MovieId = movieId,
                CreatedAt = DateTime.UtcNow,
                CommentText = commentText
            };

            _context.MovieComments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyCommentLists", "MovieComments");
        }

        public async Task<IActionResult> MyCommentLists()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var movieComments = await _context.MovieComments
                .Include(r => r.Movie)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(movieComments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(int id, MovieComment movieComment)
        {
            if (id != movieComment.Id)
                return NotFound();

            var existingMovie = await _context.MovieComments.FindAsync(id);

            if (existingMovie == null)
                return NotFound();

                // Update simple fields
                existingMovie.CommentText = movieComment.CommentText;
                existingMovie.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return RedirectToAction("MyCommentLists", "MovieComments");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the watchlist entry for this user and movie
            var item = await _context.MovieComments
                .FirstOrDefaultAsync(w => w.Id == Id && w.UserId == userId);

            if (item == null)
                return NotFound("Movie not found in your commentlist");

            _context.MovieComments.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyCommentLists));
        }
    }
}
