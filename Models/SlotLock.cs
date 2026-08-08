namespace BookingSystem.Models;

public class SlotLock
{
    public int Id { get; set; }
    public int SlotId { get; set; }
    public Slot Slot { get; set; } = null!;
    public string SessionId { get; set; } = null!;
    public DateTime LockedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}