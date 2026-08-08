using Microsoft.AspNetCore.SignalR;

namespace BookingSystemMVC.Booking.Hubs
{
    // Simple SignalR hub to broadcast reservation/booking events. Clients should join group by service+date.
    public class BookingHub : Hub
    {
        // Called by clients to subscribe to updates for a specific service on a specific date
        public async Task Subscribe(int serviceId, string date)
        {
            var group = GetGroupName(serviceId, date);
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }

        public async Task Unsubscribe(int serviceId, string date)
        {
            var group = GetGroupName(serviceId, date);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }

        private static string GetGroupName(int serviceId, string date) => $"service-{serviceId}-date-{date}";
    }
}
