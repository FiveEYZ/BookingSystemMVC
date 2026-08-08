using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookingSystemMVC.Booking.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace BookingSystemMVC.Booking.Services
{
    // Background service that periodically removes expired reservations and notifies clients via SignalR.
    public class ReservationCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly IHubContext<Hubs.BookingHub> _hub;
        private readonly ILogger<ReservationCleanupService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

        public ReservationCleanupService(IServiceProvider services, IHubContext<Hubs.BookingHub> hub, ILogger<ReservationCleanupService> logger)
        {
            _services = services;
            _hub = hub;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReservationCleanupService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredReservations(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while cleaning reservations");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("ReservationCleanupService stopping");
        }

        private async Task CleanupExpiredReservations(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

            var now = DateTime.UtcNow;
            var expired = await db.Reservations.Where(r => r.ExpiresAt <= now).ToListAsync(cancellationToken);
            if (!expired.Any()) return;

            // Group by service+date so we can notify relevant groups
            var groups = expired
                .GroupBy(r => new { r.ServiceId, Date = r.StartDateTime.Date })
                .ToList();

            db.Reservations.RemoveRange(expired);
            await db.SaveChangesAsync(cancellationToken);

            // Notify clients for each affected group
            foreach (var g in groups)
            {
                var groupName = $"service-{g.Key.ServiceId}-date-{g.Key.Date:yyyy-MM-dd}";
                await _hub.Clients.Group(groupName).SendAsync("ExpiredReservationsRemoved", new { serviceId = g.Key.ServiceId, date = g.Key.Date }, cancellationToken);
            }

            _logger.LogInformation("Removed {Count} expired reservations", expired.Count);
        }
    }
}
