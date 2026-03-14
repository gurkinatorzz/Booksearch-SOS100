namespace RoomService.Models;

public class RoomBooking
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public string BookedBy { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
}