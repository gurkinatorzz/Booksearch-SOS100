using Microsoft.AspNetCore.Mvc;
using Booksearch.ViewModels;

namespace Booksearch.Controllers;

public class RoomsController : Controller
{
    // GET – öppnar sidan
    [HttpGet]
    public IActionResult BokaGrupprum()
    {
        return View();
    }

    // POST – när formuläret skickas
    [HttpPost]
    public IActionResult BokaGrupprum(RoomBookingVM booking)
    {
        // här kommer senare API-anrop till RoomService

        return RedirectToAction("BokaGrupprum");
    }
}