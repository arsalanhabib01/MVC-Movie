using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Movie.Data;
using MVC_Movie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVC_Movie.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            // Get expired rentals
            var expiredRentals = await _context.MovieRentals
                .Include(r => r.Movie)
                .Where(r => r.DueAt < DateTime.Now && r.Status == RentalStatus.Active)
                .ToListAsync();
            
            foreach (var rental in expiredRentals)
            {
                rental.Status = RentalStatus.Expired;
                // make the movie available for others
                rental.Movie.IsAvailable = true; 
                                                 
                //_context.MovieRentals.Remove(rental);
            }
            await _context.SaveChangesAsync();

            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movies/Search
        //public async Task<IActionResult> Search(string searchString)
        //{
        //    var Movies = await _context.Movie.ToListAsync();
        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        Movies = Movies.Where(movie => movie.Title.Contains(searchString)
        //        || movie.Genre.Contains(searchString)).ToList();
        //    }
        //    return View(Movies);
        //}

        // GET: Movies/Search
        public async Task<IActionResult> Search(string searchString, string selectedGenre)
        {
            // Dropdown values
            List<SelectListItem> genres = new List<SelectListItem>()
            {
                new SelectListItem { Text = "All Genres", Value = "" },
                new SelectListItem { Text = "Action", Value = "Action" },
                new SelectListItem { Text = "Comedy", Value = "Comedy" },
                new SelectListItem { Text = "Drama", Value = "Drama" },
                new SelectListItem { Text = "Adventure", Value = "Adventure" },
                new SelectListItem { Text = "Horror", Value = "Horror" }
            };

            ViewBag.SelectGenres = genres;

            var movies = _context.Movie.AsQueryable();

            // Filter by Genre first
            if (!string.IsNullOrEmpty(selectedGenre))
            {
                //movies = movies.Where(m => m.Genre == selectedGenre);
                movies = movies.Where(m => m.Genre.Contains(selectedGenre));
            }

            // Then filter by Title
            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(m => m.Title.Contains(searchString));
            }

            return View(await movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            Movie movie = new Movie();
            movie.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(movie);
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Description,Rating,UserId,FileForm")] Movie movie)
        {
            // Check if an image was uploaded
            if (movie.FileForm == null || movie.FileForm.Length == 0)
            {
                ModelState.AddModelError("FileForm", "Image is required.");
            }

            if (ModelState.IsValid)
            {
                if (movie.FileForm != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Path.GetFileName(movie.FileForm.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await movie.FileForm.CopyToAsync(stream);
                    }

                    movie.FileName = fileName;
                    movie.IsAvailable = true;
                }
                _context.Add(movie);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Details", new { id = movie.Id });
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Description,Rating,UserId,FileName,FileForm")] Movie movie)
        {
            if (id != movie.Id)
                return NotFound();

            var existingMovie = await _context.Movie.FindAsync(id);

            if (existingMovie == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Update simple fields
                existingMovie.Title = movie.Title;
                existingMovie.Genre = movie.Genre;
                existingMovie.Description = movie.Description;
                existingMovie.ReleaseDate = movie.ReleaseDate;
                existingMovie.Rating = movie.Rating;
                existingMovie.UserId = movie.UserId;
                existingMovie.FileForm = movie.FileForm;
                existingMovie.FileName = movie.FileName;

                // If a new file was uploaded
                if (movie.FileForm != null && movie.FileForm.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Path.GetFileName(movie.FileForm.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await movie.FileForm.CopyToAsync(stream);
                    }

                    existingMovie.FileName = fileName;
                }

                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Details", new { id });
            }

            return View(movie);
        }

        // GET: Movies/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
