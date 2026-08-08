using System;
using System.ComponentModel.DataAnnotations;

namespace BookingSystemMVC.Booking.Models
{
    public class Booking
    {
        public int Id { get; set; }

        // Identity user id (nullable if anonymous bookings are supported)
        public string UserId { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        // Start date/time of the booking (stored in UTC)
        [Required]
        public DateTime StartDateTime { get; set; }

        public int DurationMinutes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Confirmed"; // or "Cancelled"

        // Optional invoice/reference fields can be added later
    }
}
