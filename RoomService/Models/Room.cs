namespace RoomService.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }

    // En-till-många: ett rum kan ha många bokningar
    public List<RoomBooking> Bookings { get; set; } = new();
}