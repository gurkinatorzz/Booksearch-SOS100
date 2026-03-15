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
    [HttpPost]
    public async Task<ActionResult<RoomBooking>> CreateBooking(RoomBooking booking)
    {
        var conflict = await _context.RoomBookings
            .AnyAsync(b =>
                b.RoomId == booking.RoomId &&
                b.StartTime < booking.EndTime &&
                b.EndTime > booking.StartTime);

        if (conflict)
        {
            return BadRequest("Room is already booked for this time.");
        }

        _context.RoomBookings.Add(booking);
        await _context.SaveChangesAsync();

        return booking;
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
}