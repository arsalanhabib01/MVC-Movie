using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Movie.Data;
using MVC_Movie.Models;
using System.Security.Claims;

namespace MVC_Movie.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminDashboardController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var dashboard = new AdminDashboardVM
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalMovies = await _context.Movie.CountAsync(),

                ActiveRentals = await _context.MovieRentals
                    .CountAsync(r => r.Status == RentalStatus.Active),

                ReturnedRentals = await _context.MovieRentals
                    .CountAsync(r => r.Status == RentalStatus.Returned),

                ExpiredRentals = await _context.MovieRentals
                    .CountAsync(r => r.Status == RentalStatus.Expired),

                TotalPurchases = await _context.MoviePurchases.CountAsync(),

                TotalRevenue =
                    await _context.MovieRentals.SumAsync(r => r.RentalPrice) +
                    await _context.MoviePurchases.SumAsync(p => p.PurchasePrice)
            };

            return View(dashboard);
        }

        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();

            var result = new List<UserWithRoleVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserWithRoleVM
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }

            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Users));
        }

    }
}
