using System;
using System.ComponentModel.DataAnnotations;

namespace BookingSystemMVC.Booking.Models
{
    public class SlotTemplate
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        // Time of day when this slot starts (e.g. 09:00)
        [Required]
        public TimeSpan StartTime { get; set; }

        // Slot length in minutes
        public int DurationMinutes { get; set; } = 60;

        public bool IsActive { get; set; } = true;
    }
}
