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
    public ActionResult<ReservationModels[]> GetReservations()
    {
        var reservations = _dbContext.Reservations.ToArray();
        return Ok(reservations);
    }

    [HttpPost]
    public IActionResult PostReservation([FromBody] ReservationModels reservation)
    {
        _dbContext.Reservations.Add(reservation);
        _dbContext.SaveChanges();

        return Ok();
    }
}