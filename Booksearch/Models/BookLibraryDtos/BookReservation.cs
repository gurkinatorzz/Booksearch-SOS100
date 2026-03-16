namespace Booksearch.Models;

public class BookReservation
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string UserName { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;
}