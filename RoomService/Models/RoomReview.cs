namespace RoomService.Models;

public class RoomReview
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int BookingId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigationsproperties
    public Room? Room { get; set; }
    public RoomBooking? Booking { get; set; }
}