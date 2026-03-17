using Microsoft.AspNetCore.Mvc;
using Booksearch.Services;

namespace Booksearch.Controllers;

public class BookLoanController : Controller
{
    private BookLibraryService _bookLibraryService;

    public BookLoanController(BookLibraryService bookLibraryService)
    {
        _bookLibraryService = bookLibraryService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Create()
    {
        var books = await _bookLibraryService.GetBooksWithDetail();
        return View(books);
    }
}