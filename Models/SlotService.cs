using BookingSystem.Models;
using BookingSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Services;

public class SlotService
{
    private readonly AppDbContext _context;

    public SlotService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Slot>> GetAvailableSlotsAsync(int serviceId, DateTime date)
    {
        var now = DateTime.UtcNow;

        var expiredLocks = _context.SlotLocks.Where(l => l.ExpiresAt < now);
        _context.SlotLocks.RemoveRange(expiredLocks);
        await _context.SaveChangesAsync();

        var lockedSlotIds = await _context.SlotLocks
            .Select(l => l.SlotId)
            .ToListAsync();

        return await _context.Slots
            .Where(s => s.ServiceId == serviceId
                        && s.Date == date.Date
                        && !s.IsBooked
                        && !lockedSlotIds.Contains(s.Id))
            .ToListAsync();
    }

    public async Task<bool> TryLockSlotAsync(int slotId, string sessionId, int lockSeconds = 120)
    {
        var now = DateTime.UtcNow;

        var existingLock = await _context.SlotLocks
            .FirstOrDefaultAsync(l => l.SlotId == slotId && l.ExpiresAt > now);

        if (existingLock != null)
            return false;

        var slot = await _context.Slots.FindAsync(slotId);
        if (slot == null || slot.IsBooked)
            return false;

        var lockEntity = new SlotLock
        {
            SlotId = slotId,
            SessionId = sessionId,
            LockedAt = now,
            ExpiresAt = now.AddSeconds(lockSeconds)
        };

        _context.SlotLocks.Add(lockEntity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task ReleaseLockAsync(int slotId, string sessionId)
    {
        var lockEntity = await _context.SlotLocks
            .FirstOrDefaultAsync(l => l.SlotId == slotId && l.SessionId == sessionId);

        if (lockEntity != null)
        {
            _context.SlotLocks.Remove(lockEntity);
            await _context.SaveChangesAsync();
        }
    }
}