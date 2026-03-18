namespace Booksearch.ViewModels;

public class RoomBookingVM
{
    public int RoomId { get; set; }
    public string BookedBy { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; } = DateTime.Today;
    public string TimeSlot { get; set; } = string.Empty;
    public List<RoomOption> Rooms { get; set; } = new();
    public List<TimeSlotOption> TimeSlots { get; set; } = new();
}

public class RoomOption
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
}

public class TimeSlotOption
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool IsBooked { get; set; }
}

public class RoomBookingListVM
{
    public int Id { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public string BookedBy { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "Aktiv";
    public bool IsHistory => EndTime < DateTime.Now || Status == "Avbokad";
}