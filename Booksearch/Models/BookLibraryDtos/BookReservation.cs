namespace Booksearch.Models;

public class BookReservation
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string UserName { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsComplete { get; set; }
    public string? Title { get; set; }
    public int QueuePosition { get; set; }
}