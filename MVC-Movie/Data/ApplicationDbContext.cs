using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC_Movie.Models;

namespace MVC_Movie.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MVC_Movie.Models.Movie> Movie { get; set; } = default!;
        public DbSet<MovieRental> MovieRentals { get; set; } = default!;
        public DbSet<MoviePurchase> MoviePurchases { get; set; } = default!;
        public DbSet<WatchList> WatchLists { get; set; } = default!;
        public DbSet<MovieComment> MovieComments { get; set; } = default!;
        public DbSet<UserProfile> UserProfiles { get; set; } = default!;
        public DbSet<MovieActor> MovieActors { get; set; } = default!;
        public DbSet<Notification> Notifications { get; set; } = default!;
        public DbSet<Coupon> Coupons { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // VERY IMPORTANT

            // Your custom table mapping
            modelBuilder.Entity<Notification>()
                .ToTable("PurchaseNotifications");
        }
    }
}
