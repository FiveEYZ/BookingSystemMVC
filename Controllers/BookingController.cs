using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookingSystemMVC.Booking.Data;
using BookingSystemMVC.Booking.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BookingSystemMVC.Controllers
{
    public class BookingController : Controller
    {
        private readonly BookingDbContext _db;
        private readonly IReservationService _reservationService;

        public BookingController(BookingDbContext db, IReservationService reservationService)
        {
            _db = db;
            _reservationService = reservationService;
        }

        // GET: /Booking
        public async Task<IActionResult> Index()
        {
            var services = await _db.Services.ToListAsync();
            return View(services);
        }

        // GET: /Booking/Service/5?date=2026-08-08
        public async Task<IActionResult> Service(int id, DateTime? date)
        {
            var svc = await _db.Services.Include(s => s.SlotTemplates).FirstOrDefaultAsync(s => s.Id == id);
            if (svc == null) return NotFound();

            var viewDate = date?.Date ?? DateTime.UtcNow.Date;

            var slots = await _reservationService.GetAvailableSlotsAsync(id, viewDate);

            ViewBag.Service = svc;
            ViewBag.Date = viewDate;
            return View(slots);
        }

        // API: GET /api/booking/{serviceId}/slots?date=2026-08-08
        [HttpGet]
        [Route("api/booking/{serviceId}/slots")]
        public async Task<IActionResult> GetSlots(int serviceId, [FromQuery] DateTime date)
        {
            var slots = await _reservationService.GetAvailableSlotsAsync(serviceId, date.Date);
            return Ok(slots);
        }

        // API: POST /api/booking/reserve
        [HttpPost]
        [Route("api/booking/reserve")]
        public async Task<IActionResult> Reserve([FromBody] ReserveRequest req)
        {
            if (req == null) return BadRequest();

            // Use session id or fallback to anonymous GUID cookie
            var sessionId = HttpContext.Session?.Id;
            if (string.IsNullOrEmpty(sessionId))
            {
                // fallback: use a cookie
                sessionId = Request.Cookies["booking-session"];
                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();
                    Response.Cookies.Append("booking-session", sessionId);
                }
            }

            var startUtc = req.StartDateTime.ToUniversalTime();
            var result = await _reservationService.ReserveAsync(sessionId, req.ServiceId, startUtc, TimeSpan.FromMinutes(10));
            if (!result.Ok) return Conflict(new { message = result.ErrorMessage });
            return Ok(new { reservationId = result.ReservationId, expiresAt = result.ExpiresAt });
        }

        // API: POST /api/booking/confirm
        [HttpPost]
        [Route("api/booking/confirm")]
        [Authorize]
        public async Task<IActionResult> Confirm([FromBody] ConfirmRequest req)
        {
            if (req == null) return BadRequest();

            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var ok = await _reservation_service_confirm_helper(req.ReservationId, userId);
            if (!ok) return BadRequest(new { message = "Could not confirm reservation." });
            return Ok();
        }

        // Helper to keep transaction/logic in service
        private async Task<bool> _reservation_service_confirm_helper(Guid reservationId, string userId)
        {
            return await _reservationService.ConfirmReservationAsync(reservationId, userId);
        }

        // API: POST /api/booking/release
        [HttpPost]
        [Route("api/booking/release")]
        public async Task<IActionResult> Release([FromBody] ReleaseRequest req)
        {
            if (req == null) return BadRequest();
            var ok = await _reservationService.ReleaseReservationAsync(req.ReservationId);
            if (!ok) return NotFound();
            return Ok();
        }

        // DTOs
        public record ReserveRequest(int ServiceId, DateTime StartDateTime);
        public record ConfirmRequest(Guid ReservationId);
        public record ReleaseRequest(Guid ReservationId);
    }
}
