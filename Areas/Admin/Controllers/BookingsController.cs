using BookingSystemMVC.Booking.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystemMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookingsController : Controller
    {
        private readonly BookingDbContext _db;

        public BookingsController(BookingDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _db.Bookings.Include(b => b.Service).ToListAsync();
            return View(bookings);
        }
    }
}
