using Microsoft.AspNetCore.Mvc;

namespace Booksearch.Controllers;

public class BookLoanController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }
}