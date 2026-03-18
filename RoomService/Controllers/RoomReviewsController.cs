using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomService.Data;
using RoomService.Models;

namespace RoomService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomReviewsController : ControllerBase
{
    private readonly RoomDbContext _context;

    public RoomReviewsController(RoomDbContext context)
    {
        _context = context;
    }

    // GET: api/roomreviews
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomReview>>> GetReviews()
    {
        return await _context.RoomReviews.ToListAsync();
    }

    // GET: api/roomreviews/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RoomReview>> GetReview(int id)
    {
        var review = await _context.RoomReviews.FindAsync(id);
        if (review == null) return NotFound();
        return review;
    }

    // POST: api/roomreviews
    [HttpPost]
    public async Task<ActionResult<RoomReview>> CreateReview(RoomReview review)
    {
        review.CreatedAt = DateTime.Now;
        _context.RoomReviews.Add(review);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
    }

    // PUT: api/roomreviews/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, RoomReview review)
    {
        if (id != review.Id) return BadRequest();

        var existing = await _context.RoomReviews.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Rating = review.Rating;
        existing.Comment = review.Comment;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/roomreviews/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await _context.RoomReviews.FindAsync(id);
        if (review == null) return NotFound();

        _context.RoomReviews.Remove(review);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}