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
}