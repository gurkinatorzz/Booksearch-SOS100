using Microsoft.AspNetCore.Mvc;

namespace Booksearch.Controllers;

public class RoomsController : Controller
{
    // GET
    public IActionResult BokaGrupprum()
    {
        return View();
    }
}