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
                    await _context.MoviePurchases.SumAsync(p => p.PurchasePrice),

                TotalDiscount =
                    await _context.MovieRentals.SumAsync(r => r.DiscountApplied) +
                    await _context.MoviePurchases.SumAsync(p => p.DiscountApplied)
            };

            return View(dashboard);
        }

        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            var profiles = await _context.UserProfiles.ToListAsync();

            var result = new List<UserWithRoleVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var profile = profiles.FirstOrDefault(p => p.UserId == user.Id);

                if (profile != null)
                {
                    await CheckBirthdayDiscount(user.Id);
                }

                result.Add(new UserWithRoleVM
                {
                    FullName = profile?.FullName ?? "No Profile",
                    DateOfBirth = profile?.DateOfBirth,
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

        private async Task CheckBirthdayDiscount(string userId)
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile?.DateOfBirth != null)
            {
                var today = DateTime.UtcNow;

                if (profile.DateOfBirth.Value.Month == today.Month &&
                    profile.DateOfBirth.Value.Day == today.Day)
                {
                    bool alreadyGiven = await _context.Coupons
                        .AnyAsync(c => c.UserId == userId &&
                                       c.Code.StartsWith("BD"));

                    if (!alreadyGiven)
                    {
                        string code = "BD" + GenerateCouponCode();

                        var coupon = new Coupon
                        {
                            Code = code,
                            DiscountPercentage = 80,
                            ExpiryDate = DateTime.UtcNow.AddDays(7),
                            CouponType = CouponType.Birthday,
                            IsActive = true,
                            UsageLimit = 1,
                            TimesUsed = 0,
                            UserId = userId
                        };

                        _context.Coupons.Add(coupon);

                        var notification = new Notification
                        {
                            UserId = userId,
                            Message = "🎂 Happy Birthday!\n" + 
                                      $"Enjoy 80% off! Coupon: {code}",
                            CreatedAt = DateTime.UtcNow,
                            IsRead = false
                        };

                        _context.Notifications.Add(notification);

                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
        private string GenerateCouponCode()
        {
            return Guid.NewGuid().ToString()
                .Substring(0, 8)
                .ToUpper();
        }

    }
}
