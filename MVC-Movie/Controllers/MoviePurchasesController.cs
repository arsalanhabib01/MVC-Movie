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
        public async Task<IActionResult> BuyMovie(int movieId, string? couponCode)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var movie = await _context.Movie.FindAsync(movieId);

            if (movie == null)
                return NotFound("Movie not found");

            bool alreadyPurchased = await _context.MoviePurchases
                .AnyAsync(p => p.MovieId == movieId && p.UserId == userId);

            if (alreadyPurchased)
                return BadRequest("Already purchased");


            // 🎯 LOYALTY SYSTEM STARTS HERE
            var completedPurchases = await _context.MoviePurchases
                .CountAsync(r => r.UserId == userId);
                    
            var nextPurchaseNumber = completedPurchases + 1;
            decimal price = 30;
            decimal discountAmount = 0;

            // 🎉 Reward ONLY at first 3 and 5 purchases
            if (nextPurchaseNumber == 3 || nextPurchaseNumber == 5)
            {
                string code = GenerateCouponCode();

                var coupon = new Coupon
                {
                    Code = code,
                    DiscountPercentage = nextPurchaseNumber == 3 ? 30 : 50,
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                    IsActive = true,
                    UsageLimit = 1,
                    TimesUsed = 0,
                    UserId = userId,
                    CouponType = CouponType.Purchase
                };
                _context.Coupons.Add(coupon);

                // 🔔 ADD NOTIFICATION HERE
                var rewardNotification = new Notification
                {
                    UserId = userId,
                    Message = "🎉 Loyalty Reward!\n" +
                              $"Congratulations! You completed {nextPurchaseNumber} purchases.\n" +
                              $"Here is your FREE purchase coupon: {code}",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };
                _context.Notifications.Add(rewardNotification);

                await _context.SaveChangesAsync();
            }

            // Apply coupon if provided
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c =>
                        c.Code == couponCode &&
                        c.IsActive &&
                        c.ExpiryDate > DateTime.UtcNow &&
                        (c.UserId == null || c.UserId == userId) &&
                        (c.CouponType == CouponType.Purchase || c.CouponType == CouponType.Birthday) &&
                        c.TimesUsed < c.UsageLimit);

                if (coupon != null)
                {
                    discountAmount = price * (coupon.DiscountPercentage / 100);
                    price -= discountAmount;

                    coupon.TimesUsed++;

                    // If coupon limit is reached 
                    if (coupon.TimesUsed >= coupon.UsageLimit)
                    {
                        coupon.IsActive = false;

                        // 🔔 ADD NOTIFICATION HERE
                        var limitNotification = new Notification
                        {
                            UserId = userId,
                            Message = $"🔔 The coupon '{coupon.Code}' has reached its usage limit and is now deactivated.",
                            CreatedAt = DateTime.Now,
                            IsRead = false
                        };
                        _context.Notifications.Add(limitNotification);
                    }
                }
            }

            // 🔔 ADD NOTIFICATION HERE
            var purchasedNotification = new Notification
            {
                UserId = userId,
                Message = $"You successfully purchased '{movie.Title}' 🎬",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(purchasedNotification);

            var purchase = new MoviePurchase
            {
                MovieId = movieId,
                UserId = userId,
                PurchaseDate = DateTime.UtcNow,
                Status = PurchaseStatus.Completed,
                PurchasePrice = (int)price,
                DiscountApplied = discountAmount,
                CouponCode = couponCode
            };
            _context.MoviePurchases.Add(purchase);

            await _context.SaveChangesAsync();

            return RedirectToAction("MyPurchases", "MoviePurchases");
        }

        private string GenerateCouponCode()
        {
            return "LOYALTY-" + Guid.NewGuid()
                .ToString()
                .Substring(0, 6)
                .ToUpper();
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
