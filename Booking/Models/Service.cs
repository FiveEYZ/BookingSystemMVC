using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingSystemMVC.Booking.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public ICollection<SlotTemplate> SlotTemplates { get; set; }
    }
}
