using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomService.Data;
using RoomService.Models;

namespace RoomService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomBookingsController : ControllerBase
{
    private readonly RoomDbContext _context;

    public RoomBookingsController(RoomDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomBooking>>> GetBookings()
    {
        return await _context.RoomBookings.ToListAsync();
    }

   
    [HttpPost]
    public async Task<ActionResult<RoomBooking>> CreateBooking(RoomBooking booking)
    {
        var conflict = await _context.RoomBookings
            .AnyAsync(b =>
                b.RoomId == booking.RoomId &&
                b.StartTime < booking.EndTime &&
                b.EndTime > booking.StartTime &&
                b.Status != "Avbokad");

        if (conflict)
        {
            return BadRequest("Room is already booked for this time.");
        }

        _context.RoomBookings.Add(booking);
        await _context.SaveChangesAsync();

        return booking;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking(int id, RoomBooking booking)
    {
        if (id != booking.Id)
            return BadRequest();

        var existing = await _context.RoomBookings.FindAsync(id);
        if (existing == null)
            return NotFound();

        existing.RoomId    = booking.RoomId;
        existing.BookedBy  = booking.BookedBy;
        existing.StartTime = booking.StartTime;
        existing.EndTime   = booking.EndTime;
        existing.Purpose   = booking.Purpose;

        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _context.RoomBookings.FindAsync(id);

        if (booking == null)
            return NotFound();

        _context.RoomBookings.Remove(booking);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpPut("avboka/{id}")]
    public async Task<IActionResult> AvbokaBooking(int id)
    {
        var booking = await _context.RoomBookings.FindAsync(id);
        if (booking == null) return NotFound();
    
        booking.Status = "Avbokad";
        await _context.SaveChangesAsync();
        return NoContent();
    }
}