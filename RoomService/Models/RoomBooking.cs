
namespace RoomService.Models;

public class RoomBooking
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string BookedBy { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    // Navigeringsproperty — kopplingen till Room-tabellen
    public Room Room { get; set; } = null!;
}