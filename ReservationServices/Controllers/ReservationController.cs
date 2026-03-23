using Microsoft.AspNetCore.Mvc;
using ReservationServices.Data;
using ReservationServices.Models;

namespace ReservationServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly ReservationServiceDbContext _dbContext;

    public ReservationController(ReservationServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ReservationModels>> GetReservations()
    {
        var reservations = _dbContext.Reservations
            .OrderBy(r => r.CreatedAtUtc)
            .ToList();

        return Ok(reservations);
    }

    [HttpGet("{bookId}")]
    public ActionResult<IEnumerable<ReservationModels>> GetReservationsByBookId(int bookId)
    {
        var reservations = _dbContext.Reservations
            .Where(r => r.BookId == bookId && !r.IsComplete)
            .OrderBy(r => r.CreatedAtUtc)
            .ToList();

        return Ok(reservations);
    }

    [HttpPost]
    public IActionResult PostReservation([FromBody] ReservationModels reservation)
    {
        reservation.UserName = (reservation.UserName ?? "").Trim();

        if (string.IsNullOrWhiteSpace(reservation.UserName))
            return BadRequest("Namn måste fyllas i.");

        var alreadyExists = _dbContext.Reservations.Any(r =>
            r.BookId == reservation.BookId &&
            !r.IsComplete &&
            r.UserName.ToLower() == reservation.UserName.ToLower());

        if (alreadyExists)
            return BadRequest("Du står redan i kö för den här boken.");

        reservation.CreatedAtUtc = DateTime.UtcNow;
        reservation.IsComplete = false;

        _dbContext.Reservations.Add(reservation);
        _dbContext.SaveChanges();

        return Ok(reservation);
    }

    [HttpDelete("{id}")]
    public IActionResult CancelReservation(int id)
    {
        var reservation = _dbContext.Reservations.FirstOrDefault(r => r.Id == id);

        if (reservation == null)
            return NotFound();

        reservation.IsComplete = true;
        _dbContext.SaveChanges();

        return NoContent();
    }
}