using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookingSystemMVC.Booking.Data;
using BookingSystemMVC.Booking.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BookingSystemMVC.Booking.Services
{
    public class ReservationService : IReservationService
    {
        private readonly BookingDbContext _db;
        private readonly IHubContext<Hubs.BookingHub> _hub;

        public ReservationService(BookingDbContext db, IHubContext<Hubs.BookingHub> hub)
        {
            _db = db;
            _hub = hub;
        }

        // Returns slots for a given date (server-side date, StartDateTime's Date equals provided date.Date)
        public async Task<IEnumerable<SlotDto>> GetAvailableSlotsAsync(int serviceId, DateTime date, CancellationToken cancellationToken = default)
        {
            var dayStart = date.Date;
            var dayEnd = dayStart.AddDays(1);

            // Get slot templates for the service
            var templates = await _db.SlotTemplates
                .Where(t => t.ServiceId == serviceId && t.IsActive)
                .ToListAsync(cancellationToken);

            // Get bookings and active reservations for the date
            var bookings = await _db.Bookings
                .Where(b => b.ServiceId == serviceId && b.StartDateTime >= dayStart && b.StartDateTime < dayEnd)
                .Select(b => b.StartDateTime)
                .ToListAsync(cancellationToken);

            var now = DateTime.UtcNow;
            var reservations = await _db.Reservations
                .Where(r => r.ServiceId == serviceId && r.StartDateTime >= dayStart && r.StartDateTime < dayEnd && r.ExpiresAt > now)
                .ToListAsync(cancellationToken);

            var result = new List<SlotDto>();
            foreach (var t in templates)
            {
                var start = dayStart + t.StartTime;
                var dto = new SlotDto
                {
                    StartDateTime = start,
                    DurationMinutes = t.DurationMinutes
                };

                if (bookings.Contains(start))
                {
                    dto.IsAvailable = false;
                }
                else
                {
                    var res = reservations.FirstOrDefault(r => r.StartDateTime == start);
                    if (res != null)
                    {
                        dto.IsAvailable = false;
                        dto.ReservationId = res.Id;
                    }
                    else
                    {
                        dto.IsAvailable = true;
                    }
                }

                result.Add(dto);
            }

            return result.OrderBy(s => s.StartDateTime);
        }

        public async Task<ReserveResult> ReserveAsync(string sessionId, int serviceId, DateTime startDateTime, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            // Remove expired reservations (simple housekeeping)
            var expired = await _db.Reservations.Where(r => r.ExpiresAt <= now).ToListAsync(cancellationToken);
            if (expired.Any())
            {
                _db.Reservations.RemoveRange(expired);
                await _db.SaveChangesAsync(cancellationToken);
            }

            // Check for existing booking
            var existsBooking = await _db.Bookings.AnyAsync(b => b.ServiceId == serviceId && b.StartDateTime == startDateTime, cancellationToken);
            if (existsBooking) return new ReserveResult(false, null, null, "Slot already booked");

            // Check for active reservation
            var existsRes = await _db.Reservations.AnyAsync(r => r.ServiceId == serviceId && r.StartDateTime == startDateTime && r.ExpiresAt > now, cancellationToken);
            if (existsRes) return new ReserveResult(false, null, null, "Slot temporarily reserved by somebody else");

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                SessionId = sessionId,
                ServiceId = serviceId,
                StartDateTime = startDateTime,
                CreatedAt = now,
                ExpiresAt = now.Add(timeout)
            };

            _db.Reservations.Add(reservation);
            await _db.SaveChangesAsync(cancellationToken);

            // Notify via SignalR that a slot changed (so other clients can update)
            await _hub.Clients.Group(GetGroupName(serviceId, startDateTime.Date)).SendAsync("SlotReserved", new { serviceId, startDateTime, reservationId = reservation.Id });

            return new ReserveResult(true, reservation.Id, reservation.ExpiresAt);
        }

        public async Task<bool> ConfirmReservationAsync(Guid reservationId, string userId, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var reservation = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken);
            if (reservation == null) return false;
            if (reservation.ExpiresAt <= now) return false;

            // Double-check no booking exists
            var hasBooking = await _db.Bookings.AnyAsync(b => b.ServiceId == reservation.ServiceId && b.StartDateTime == reservation.StartDateTime, cancellationToken);
            if (hasBooking) return false;

            // Create booking in a transaction
            using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var booking = new Booking
                {
                    UserId = userId,
                    ServiceId = reservation.ServiceId,
                    StartDateTime = reservation.StartDateTime,
                    DurationMinutes = 60,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Confirmed"
                };
                _db.Bookings.Add(booking);

                // remove reservation
                _db.Reservations.Remove(reservation);

                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);

                // Notify clients about booked slot
                await _hub.Clients.Group(GetGroupName(reservation.ServiceId, reservation.StartDateTime.Date)).SendAsync("SlotBooked", new { reservation.ServiceId, reservation.StartDateTime });

                return true;
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<bool> ReleaseReservationAsync(Guid reservationId, CancellationToken cancellationToken = default)
        {
            var reservation = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken);
            if (reservation == null) return false;

            _db.Reservations.Remove(reservation);
            await _db.SaveChangesAsync(cancellationToken);

            // Notify clients that slot is released
            await _hub.Clients.Group(GetGroupName(reservation.ServiceId, reservation.StartDateTime.Date)).SendAsync("SlotReleased", new { reservation.ServiceId, reservation.StartDateTime });

            return true;
        }

        private static string GetGroupName(int serviceId, DateTime date) => $"service-{serviceId}-date-{date:yyyy-MM-dd}";
    }
}
