using System.Threading.Tasks;
using BookingSystemMVC.Booking.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingSystemMVC.Controllers
{
    [Authorize]
    public class UserBookingsController : Controller
    {
        private readonly BookingDbContext _db;

        public UserBookingsController(BookingDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _db.Bookings
                .Include(b => b.Service)
                .Where(b => b.UserId == userId)
                .ToListAsync();
            return View(bookings);
        }
    }
}
