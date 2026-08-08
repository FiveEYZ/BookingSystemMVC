using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BookingSystemMVC.Booking.Models;

namespace BookingSystemMVC.Booking.Services
{
    public class SlotDto
    {
        public DateTime StartDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsAvailable { get; set; }
        public Guid? ReservationId { get; set; }
    }

    public record ReserveResult(bool Ok, Guid? ReservationId, DateTime? ExpiresAt, string ErrorMessage = null);

    public interface IReservationService
    {
        Task<IEnumerable<SlotDto>> GetAvailableSlotsAsync(int serviceId, DateTime date, CancellationToken cancellationToken = default);
        Task<ReserveResult> ReserveAsync(string sessionId, int serviceId, DateTime startDateTime, TimeSpan timeout, CancellationToken cancellationToken = default);
        Task<bool> ConfirmReservationAsync(Guid reservationId, string userId, CancellationToken cancellationToken = default);
        Task<bool> ReleaseReservationAsync(Guid reservationId, CancellationToken cancellationToken = default);
    }
}
