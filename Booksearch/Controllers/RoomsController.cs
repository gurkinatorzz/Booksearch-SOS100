using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Booksearch.ViewModels;
using System.Net.Http;
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
    public async Task<IActionResult> BokaGrupprum()
    {
        var rooms = new List<RoomOption>();

        try
        {
            var response = await _httpClient.GetAsync("api/rooms");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                rooms = JsonSerializer.Deserialize<List<RoomOption>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new();
            }
        }
        catch
        {
            rooms = new List<RoomOption>
            {
                new() { Id = 1, Name = "London", Capacity = 4 },
                new() { Id = 2, Name = "Rom",    Capacity = 6 },
                new() { Id = 3, Name = "Paris",  Capacity = 8 }
            };
        }

        var vm = new RoomBookingVM
        {
            BookedBy  = User.Identity?.Name ?? "Gäst",
            Rooms     = rooms,
            TimeSlots = new List<TimeSlotOption>
            {
                new() { Value = "10:00", Label = "10:00–11:00", IsBooked = false },
                new() { Value = "12:00", Label = "12:00–13:00", IsBooked = false },
                new() { Value = "14:00", Label = "14:00–15:00", IsBooked = false }
            }
        };

        return View(vm);
    }

    // POST - skickar bokning
    [HttpPost]
    public async Task<IActionResult> BokaGrupprum(RoomBookingVM vm)
    {
        var booking = new
        {
            RoomId    = vm.RoomId,
            BookedBy  = vm.BookedBy,
            StartTime = vm.BookingDate.Date + TimeSpan.Parse(vm.TimeSlot),
            EndTime   = vm.BookingDate.Date + TimeSpan.Parse(vm.TimeSlot) + TimeSpan.FromHours(1)
        };

        var json    = JsonSerializer.Serialize(booking);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await _httpClient.PostAsync("api/roombookings", content);

        return RedirectToAction("MinaBokningar");
    }

    // GET - visa bokningar
    public async Task<IActionResult> MinaBokningar()
    {
        var bookings = new List<RoomBookingListVM>();

        try
        {
            var response = await _httpClient.GetAsync("api/roombookings");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                bookings = JsonSerializer.Deserialize<List<RoomBookingListVM>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new();
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
        await _httpClient.DeleteAsync($"api/roombookings/{id}");
        return RedirectToAction("MinaBokningar");
    }
}