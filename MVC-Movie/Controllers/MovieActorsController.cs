using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Tasks;
using Microsoft.EntityFrameworkCore;
using MVC_Movie.Data;
using MVC_Movie.Models;
using System.Security.Claims;

namespace MVC_Movie.Controllers
{
    [Authorize]
    public class MovieActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MovieActorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 GET: Actors/Details/5
        [HttpGet]
        public async Task<IActionResult> ActorDetails(int id)
        {
            var actor = await _context.MovieActors
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null)
                return NotFound();

            return View(actor);
        }

        // 🔹 GET: Actors/Create
        [HttpGet]
        public IActionResult AddActorProfile()
        {
            ViewData["MovieId"] = new SelectList(_context.Movie, "Id", "Title");
            return View();
        }

        // 🔹 POST: MovieActors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActorProfile(MovieActor movieActor)
        {
            // Upload Image
            if (movieActor.ImageFile != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Actor/images");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Path.GetFileName(movieActor.ImageFile.FileName);

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await movieActor.ImageFile.CopyToAsync(stream);
                    }

                    movieActor.ProfileImage = fileName;
                }

                _context.MovieActors.Add(movieActor);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Movies", new { id = movieActor.MovieId });
        }

        // GET: Edit/5
        [HttpGet]
        public async Task<IActionResult> EditActorProfile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var existingActorProfile = await _context.MovieActors.FindAsync(id);
            if (existingActorProfile == null)
            {
                return NotFound();
            }
            return View(existingActorProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditActorProfile(int id, MovieActor movieActor)
        {
            if (id != movieActor.Id)
                return NotFound();

            var existingActorProfile = await _context.MovieActors.FindAsync(id);

            if (existingActorProfile == null)
                return NotFound();

            // Update simple fields
            existingActorProfile.FullName = movieActor.FullName;
            existingActorProfile.DateOfBirth = movieActor.DateOfBirth;
            existingActorProfile.Nationality = movieActor.Nationality;
            existingActorProfile.Biography = movieActor.Biography;

            // If a new file was uploaded
            if (movieActor.ImageFile != null && movieActor.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Actor/images");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Path.GetFileName(movieActor.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await movieActor.ImageFile.CopyToAsync(stream);
                }

                existingActorProfile.ProfileImage = fileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ActorDetails", new { id = movieActor.Id });
        }

        // GET: Movies/Delete/5
        //[Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> DeleteActorProfile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieActor = await _context.MovieActors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movieActor == null)
            {
                return NotFound();
            }

            return View(movieActor);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("DeleteActorProfile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movieActor = await _context.MovieActors.FindAsync(id);
            if (movieActor != null)
            {
                _context.MovieActors.Remove(movieActor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Movies", new { id = movieActor?.MovieId });

        }

    }
}
