using BookingSystemMVC.Booking.Data;
using BookingSystemMVC.Booking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystemMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SlotTemplatesController : Controller
    {
        private readonly BookingDbContext _db;

        public SlotTemplatesController(BookingDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int serviceId)
        {
            var service = await _db.Services.Include(s => s.SlotTemplates).FirstOrDefaultAsync(s => s.Id == serviceId);
            if (service == null) return NotFound();
            ViewBag.Service = service;
            return View(service.SlotTemplates);
        }

        public IActionResult Create(int serviceId)
        {
            ViewBag.ServiceId = serviceId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SlotTemplate slot)
        {
            if (!ModelState.IsValid) return View(slot);
            _db.SlotTemplates.Add(slot);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { serviceId = slot.ServiceId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var slot = await _db.SlotTemplates.FindAsync(id);
            if (slot == null) return NotFound();
            return View(slot);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SlotTemplate slot)
        {
            if (!ModelState.IsValid) return View(slot);
            _db.SlotTemplates.Update(slot);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { serviceId = slot.ServiceId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var slot = await _db.SlotTemplates.FindAsync(id);
            if (slot == null) return NotFound();
            return View(slot);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var slot = await _db.SlotTemplates.FindAsync(id);
            if (slot != null)
            {
                _db.SlotTemplates.Remove(slot);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { serviceId = slot?.ServiceId });
        }
    }
}
