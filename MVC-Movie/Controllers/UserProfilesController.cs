using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MVC_Movie.Data;
using MVC_Movie.Models;
using System.Security.Claims;

namespace MVC_Movie.Controllers
{
    [Authorize]
    public class UserProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> AddMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if profile already exists
            var existingProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingProfile != null)
                return RedirectToAction(nameof(MyProfile));

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMyProfile(UserProfile userProfile)
        {
            userProfile.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userProfile.CreatedAt = DateTime.UtcNow;

            if (userProfile.ImageFile != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profile");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Path.GetFileName(userProfile.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await userProfile.ImageFile.CopyToAsync(stream);
                }

                userProfile.ProfileImage = fileName;
            }

            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyProfile));
        }

        // GET: Edit/5
        [HttpGet]
        public async Task<IActionResult> EditMyProfile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var existingUserProfile = await _context.UserProfiles.FindAsync(id);
            if (existingUserProfile == null)
            {
                return NotFound();
            }
            return View(existingUserProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMyProfile(int id, UserProfile userProfile)
        {
            if (id != userProfile.Id)
                return NotFound();

            var existingUserProfile = await _context.UserProfiles.FindAsync(id);

            if (existingUserProfile == null)
                return NotFound();

            // Update simple fields
            existingUserProfile.FullName = userProfile.FullName;
            existingUserProfile.DateOfBirth = userProfile.DateOfBirth;
            existingUserProfile.Bio = userProfile.Bio;

            // If a new file was uploaded
            if (userProfile.ImageFile != null && userProfile.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profile");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Path.GetFileName(userProfile.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await userProfile.ImageFile.CopyToAsync(stream);
                }

                existingUserProfile.ProfileImage = fileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyProfile));
        }

        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return RedirectToAction(nameof(AddMyProfile));

            return View(profile);
        }

    }
}
