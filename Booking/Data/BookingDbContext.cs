using BookingSystemMVC.Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystemMVC.Booking.Data
{
    // Separate DbContext for booking-related tables. You can merge into your existing ApplicationDbContext if preferred.
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
        {
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<SlotTemplate> SlotTemplates { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure a booking for the same service + start time is unique
            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.ServiceId, b.StartDateTime })
                .IsUnique();

            // Index reservation lookups and ensure we can quickly find active reservations
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.ServiceId, r.StartDateTime });
        }
    }
}
