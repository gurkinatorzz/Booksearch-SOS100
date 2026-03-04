using Booksearch.Models;

namespace Booksearch.ViewModels;

public class ReservationQueueVm
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = "";
    public IReadOnlyList<BookReservation> Reservations { get; set; } = new List<BookReservation>();
}