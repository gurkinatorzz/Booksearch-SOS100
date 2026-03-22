using Booksearch.Models;

namespace Booksearch.Services;

public class ReservationService
{
    private readonly HttpClient _httpClient;

    public ReservationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BookReservation[]> GetReservations(int bookId)
    {
        var result = await _httpClient.GetFromJsonAsync<BookReservation[]>($"reservations/{bookId}");
        if (result == null)
        {
            return [];
        }
        return result;
    }
    
    // In-memory kö (enkel att komma igång med). Byt till DB senare om ni vill.
    private static readonly List<BookReservation> _reservations = new();
    private static int _nextId = 1;

    public IReadOnlyList<BookReservation> GetQueue(int bookId)
    {
        return _reservations
            .Where(r => r.BookId == bookId && r.Status == ReservationStatus.Active)
            .OrderBy(r => r.CreatedAtUtc)
            .ToList();
    }

    public BookReservation Reserve(int bookId, string userName)
    {
        userName = (userName ?? "").Trim();
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Namn måste fyllas i.");

        // Förhindra att samma person ställer sig i kö flera gånger för samma bok
        var already = _reservations.Any(r =>
            r.BookId == bookId &&
            r.Status == ReservationStatus.Active &&
            string.Equals(r.UserName, userName, StringComparison.OrdinalIgnoreCase));

        if (already)
            throw new InvalidOperationException("Du står redan i kö för den här boken.");

        var reservation = new BookReservation
        {
            Id = _nextId++,
            BookId = bookId,
            UserName = userName,
            CreatedAtUtc = DateTime.UtcNow,
            Status = ReservationStatus.Active
        };

        _reservations.Add(reservation);
        return reservation;
    }

    public void Cancel(int reservationId)
    {
        var res = _reservations.FirstOrDefault(r => r.Id == reservationId);
        if (res == null) return;

        res.Status = ReservationStatus.Cancelled;
    }
}