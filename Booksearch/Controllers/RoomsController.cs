using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Booksearch.ViewModels;
using System.Text;
using System.Text.Json;

namespace Booksearch.Controllers;

public class RoomsController : Controller
{
    private readonly HttpClient _httpClient;

    public RoomsController(IConfiguration config)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(config["RoomServiceAdress"] ?? "http://localhost:5091/");
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", config["RoomServiceApiKey"] ?? "");
    }

    // GET - bokningssidan
    [Authorize]
    public async Task<IActionResult> BokaGrupprum(string? date = null)
    {
        var selectedDate = date != null ? DateTime.Parse(date) : DateTime.Today;
        var rooms = new List<RoomOption>();
        var bookings = new List<RoomBookingListVM>();

        try
        {
            var roomsResponse = await _httpClient.GetAsync("api/rooms");
            if (roomsResponse.IsSuccessStatusCode)
            {
                var json = await roomsResponse.Content.ReadAsStringAsync();
                rooms = JsonSerializer.Deserialize<List<RoomOption>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            var bookingsResponse = await _httpClient.GetAsync("api/roombookings");
            if (bookingsResponse.IsSuccessStatusCode)
            {
                var json = await bookingsResponse.Content.ReadAsStringAsync();
                bookings = JsonSerializer.Deserialize<List<RoomBookingListVM>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
        }
        catch
        {
            rooms = new List<RoomOption>
            {
                new() { Id = 1, Name = "London", Capacity = 4 },
                new() { Id = 2, Name = "Rom", Capacity = 6 },
                new() { Id = 3, Name = "Paris", Capacity = 8 }
            };
        }

        var vm = new RoomBookingVM
        {
            BookedBy = User.Identity?.Name!,
            BookingDate = selectedDate,
            Rooms = rooms,
            Bookings = bookings,
            TimeSlots = new List<TimeSlotOption>
            {
                new() { Value = "10:00", Label = "10:00–12:00", IsBooked = false },
                new() { Value = "12:00", Label = "12:00–14:00", IsBooked = false },
                new() { Value = "14:00", Label = "14:00–16:00", IsBooked = false }
            }
        };

        return View(vm);
    }

    // POST - skapa bokning
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> BokaGrupprum(RoomBookingVM vm)
    {
        var start = DateTime.Parse($"{vm.BookingDate:yyyy-MM-dd} {vm.TimeSlot}");
        var end = start.AddHours(2);

        var booking = new
        {
            RoomId = vm.RoomId,
            BookedBy = User.Identity!.Name,
            Purpose = vm.Purpose ?? "",
            StartTime = start,
            EndTime = end
        };

        var json = JsonSerializer.Serialize(booking);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/roombookings", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Content($"Fel från API: {response.StatusCode} - {error}");
        }

        return RedirectToAction("BokningBekraftad");
    }

    // GET - bekräftelsesida
    public IActionResult BokningBekraftad()
    {
        return View();
    }

    // GET - mina bokningar
    [Authorize]
    public async Task<IActionResult> MinaBokningar()
    {
        var bookings = new List<RoomBookingListVM>();
        var rooms = new List<RoomOption>();

        try
        {
            var response = await _httpClient.GetAsync("api/roombookings");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                bookings = JsonSerializer.Deserialize<List<RoomBookingListVM>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                bookings = bookings
                    .Where(b => b.BookedBy != null &&
                                b.BookedBy.Equals(User.Identity!.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var roomsResponse = await _httpClient.GetAsync("api/rooms");
            if (roomsResponse.IsSuccessStatusCode)
            {
                var json = await roomsResponse.Content.ReadAsStringAsync();
                rooms = JsonSerializer.Deserialize<List<RoomOption>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            foreach (var b in bookings)
            {
                var room = rooms.FirstOrDefault(r => r.Id == b.RoomId);
                if (room != null) b.RoomName = room.Name;
            }
        }
        catch
        {
            // RoomService inte igång
        }

        return View(bookings);
    }

    // POST - avboka
    [HttpPost]
    public async Task<IActionResult> AvbokaGrupprum(int id)
    {
        await _httpClient.PutAsync($"api/roombookings/avboka/{id}", null);
        return RedirectToAction("BokningAvbokad");
    }

    public IActionResult BokningAvbokad()
    {
        return View();
    }
    
    // GET - alla bokningar (admin och medarbetare)
    [Authorize(Roles = "Admin,Medarbetare")]
    public async Task<IActionResult> AllaBokningar()
    {
        var bookings = new List<RoomBookingListVM>();
        var rooms = new List<RoomOption>();

        try
        {
            var response = await _httpClient.GetAsync("api/roombookings");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                bookings = JsonSerializer.Deserialize<List<RoomBookingListVM>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            var roomsResponse = await _httpClient.GetAsync("api/rooms");
            if (roomsResponse.IsSuccessStatusCode)
            {
                var json = await roomsResponse.Content.ReadAsStringAsync();
                rooms = JsonSerializer.Deserialize<List<RoomOption>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            foreach (var b in bookings)
            {
                var room = rooms.FirstOrDefault(r => r.Id == b.RoomId);
                if (room != null) b.RoomName = room.Name;
            }
        }
        catch { }

        return View(bookings);
    }

// POST - omboka
    [HttpPost]
    public async Task<IActionResult> OmbokaGrupprum(int id, string bookedBy, DateTime newDate, string newTimeSlot)
    {
        var start = newDate.Date + TimeSpan.Parse(newTimeSlot);
        var end = start.AddHours(2);

        var updated = new
        {
            Id = id,
            RoomId = 0,
            BookedBy = bookedBy,
            StartTime = start,
            EndTime = end,
            Purpose = "",
            Status = "Aktiv"
        };

        var json = JsonSerializer.Serialize(updated);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await _httpClient.PutAsync($"api/roombookings/{id}", content);

        return RedirectToAction("AllaBokningar");
    }
}
