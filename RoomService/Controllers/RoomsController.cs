using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomService.Data;
using RoomService.Models;

namespace RoomService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly RoomDbContext _context;

    public RoomsController(RoomDbContext context)
    {
        _context = context;
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return NotFound();
    
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    // GET: api/rooms
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
    {
        return await _context.Rooms.ToListAsync();
    }

    // POST: api/rooms
    [HttpPost]
    public async Task<ActionResult<Room>> CreateRoom(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRooms), new { id = room.Id }, room);
    }

    // NY METOD ↓
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<Room>>> GetAvailableRooms(DateTime startTime, DateTime endTime)
    {
        var bookedRoomIds = await _context.RoomBookings
            .Where(b => b.StartTime < endTime && b.EndTime > startTime)
            .Select(b => b.RoomId)
            .ToListAsync();

        var availableRooms = await _context.Rooms
            .Where(r => !bookedRoomIds.Contains(r.Id))
            .ToListAsync();

        return availableRooms;
    }
}