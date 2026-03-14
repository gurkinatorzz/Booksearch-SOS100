namespace Booksearch.ViewModels;

public class RoomBookingVM
{
    public int RoomId { get; set; }

    public string BookedBy { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
}