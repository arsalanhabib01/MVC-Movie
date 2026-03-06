using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Movie.Data;
using MVC_Movie.Models;
using System.Security.Claims;

namespace MVC_Movie.Controllers
{
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CouponsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult ValidateCoupon(string couponCode, string types)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var typeList = types.Split(',')
                    .Select(t => Enum.Parse<CouponType>(t))
                    .ToList();

            var coupon = _context.Coupons
                .FirstOrDefault(c => c.UserId == userId &&
                                c.Code == couponCode &&
                                typeList.Contains(c.CouponType));

            // Coupon Not Found
            if (coupon == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Coupon does not exist ❌"
                });
            }

            // Usage Limit Reached
            if (coupon.TimesUsed >= coupon.UsageLimit)
            {
                return Json(new
                {
                    success = false,
                    message = "Coupon limit exceeded 🔒"
                });
            }

            // Coupon Expired
            if (coupon.ExpiryDate < DateTime.Now)
            {
                return Json(new
                {
                    success = false,
                    message = "Coupon has expired ⛔"
                });
            }

            // Coupon Inactive
            if (!coupon.IsActive)
            {
                return Json(new
                {
                    success = false,
                    message = "Coupon is inactive 🚫"
                });
            }

            return Json(new
            {
                success = true,
                discount = coupon.DiscountPercentage,
                message = $"Discount applied! Saving {coupon.DiscountPercentage:0}% on this {coupon.CouponType}."
            });
        }

        public async Task<IActionResult> MyCoupons()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myCoupons = await _context.Coupons
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CouponType)
                .ToListAsync();

            return View(myCoupons);
        }
    }
}
