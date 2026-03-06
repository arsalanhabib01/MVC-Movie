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
        public async Task<IActionResult> RentMovie(int movieId, string? couponCode)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var movie = await _context.Movie.FindAsync(movieId);

            if (movie == null)
                return NotFound("Movie not found");

            decimal price = 8;
            decimal discountAmount = 0;

            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = await _context.Coupons
                    .FirstOrDefaultAsync(c =>
                        c.Code == couponCode &&
                        c.IsActive &&
                        c.ExpiryDate > DateTime.UtcNow &&
                        (c.UserId == null || c.UserId == userId) &&
                        (c.CouponType == CouponType.Rent || c.CouponType == CouponType.Birthday) &&
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

            var rental = new MovieRental
            {
                MovieId = movieId,
                UserId = userId,
                RentedAt = DateTime.UtcNow,
                DueAt = DateTime.UtcNow.AddDays(3),
                Status = RentalStatus.Active,
                RentalPrice = (int)price,
                DiscountApplied = discountAmount,
                CouponCode = couponCode,
            };

            movie.IsAvailable = false;

            _context.MovieRentals.Add(rental);

            // 🔔 ADD NOTIFICATION HERE
            var rentalNotification = new Notification
            {
                UserId = userId,
                Message = $"You successfully rented '{movie.Title}' 🎬. \n" +
                          $"Please return it by {rental.DueAt.ToLocalTime():MMM dd, yyyy hh:mm tt}.",
                CreatedAt = DateTime.Now,
                IsRead = false
            };
            _context.Notifications.Add(rentalNotification);

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

            // 🔔 ADD NOTIFICATION HERE
            var returnedNotification = new Notification
            {
                UserId = rental.UserId,
                Message = $"Return Confirmed ✅ '{rental.Movie.Title}' has been returned successfully.\n" +
                          $"We hope you enjoyed watching it! 🍿",
                CreatedAt = DateTime.Now,
                IsRead = false
            };
            _context.Notifications.Add(returnedNotification);

            await _context.SaveChangesAsync();

            // 🎯 LOYALTY SYSTEM STARTS HERE
            var userId = rental.UserId;

            var completedRentals = await _context.MovieRentals
                .CountAsync(r => r.UserId == userId &&
                                 r.Status == RentalStatus.Returned);

            // 🎉 Reward ONLY at first 5 and 10 rentals
            if (completedRentals == 5 || completedRentals == 10)
            {
                string code = GenerateCouponCode();

                var coupon = new Coupon
                {
                    Code = code,
                    DiscountPercentage = completedRentals == 5 ? 50 : 100,
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                    IsActive = true,
                    UsageLimit = 2,
                    TimesUsed = 0,
                    UserId = userId,
                    CouponType = CouponType.Rent
                };
                _context.Coupons.Add(coupon);

                // 🔔 ADD NOTIFICATION HERE
                var rewardNotification = new Notification
                {
                    UserId = userId,
                    Message = "🎉 Loyalty Reward!\n" + 
                              $"Congratulations! You completed {completedRentals} rentals.\n" +
                              $"Here is your FREE rental coupon: {code}",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };
                _context.Notifications.Add(rewardNotification);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MyRentals));
        }

        private string GenerateCouponCode()
        {
            return "LOYALTY-" + Guid.NewGuid()
                .ToString()
                .Substring(0, 6)
                .ToUpper();
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
