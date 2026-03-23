namespace ReservationServices.Models;

public class ReservationModels
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string UserName { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string? Title { get; set; }
    public bool IsComplete { get; set; }
}