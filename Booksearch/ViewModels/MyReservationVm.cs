namespace Booksearch.ViewModels;

public class MyReservationVm
{
    public int ReservationId { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = "";
    public string UserName { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
}