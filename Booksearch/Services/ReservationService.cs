using System.Net.Http.Json;
using Booksearch.Models;

namespace Booksearch.Services;

public class ReservationService
{
    private readonly HttpClient _httpClient;

    public ReservationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<BookReservation>> GetQueue(int bookId)
    {
        var result = await _httpClient.GetFromJsonAsync<BookReservation[]>($"api/reservation/{bookId}");
        return result ?? [];
    }

    public async Task<IReadOnlyList<BookReservation>> GetMyReservations(string userName)
    {
        var result = await _httpClient.GetFromJsonAsync<BookReservation[]>($"api/reservation/user/{userName}");
        return result ?? [];
    }

    public async Task Reserve(int bookId, string userName)
    {
        userName = (userName ?? "").Trim();

        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Namn måste fyllas i.");

        var queue = await GetQueue(bookId);

        var alreadyExists = queue.Any(r =>
            !r.IsComplete &&
            string.Equals(r.UserName, userName, StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
            throw new InvalidOperationException("Du står redan i kö för den här boken.");

        var reservation = new BookReservation
        {
            BookId = bookId,
            UserName = userName,
            CreatedAtUtc = DateTime.UtcNow,
            IsComplete = false
        };

        var response = await _httpClient.PostAsJsonAsync("api/reservation", reservation);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(string.IsNullOrWhiteSpace(error)
                ? "Det gick inte att skapa reservationen."
                : error);
        }
    }

    public async Task Cancel(int reservationId)
    {
        var response = await _httpClient.DeleteAsync($"api/reservation/{reservationId}");

        if (!response.IsSuccessStatusCode)
            throw new Exception("Det gick inte att avboka reservationen.");
    }
}