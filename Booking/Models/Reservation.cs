using System;
using System.ComponentModel.DataAnnotations;

namespace BookingSystemMVC.Booking.Models
{
    public class Reservation
    {
        [Key]
        public Guid Id { get; set; }

        // SessionId (or anonymous token) to identify the reservering owner on the client
        public string SessionId { get; set; }

        public int ServiceId { get; set; }

        // Start date/time of the reserved slot (stored in UTC)
        [Required]
        public DateTime StartDateTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // When the temporary reservation expires
        public DateTime ExpiresAt { get; set; }
    }
}
