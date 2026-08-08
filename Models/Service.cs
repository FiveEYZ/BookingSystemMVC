namespace BookingSystem.Models;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int DefaultSlotLengthMinutes { get; set; }
    public ICollection<Slot> Slots { get; set; } = new List<Slot>();
}