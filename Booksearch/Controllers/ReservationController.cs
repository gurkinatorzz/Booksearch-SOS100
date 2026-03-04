using Booksearch.Services;
using Booksearch.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Booksearch.Controllers;

public class ReservationController : Controller
{
    private readonly BookLibraryService _bookLibrary;
    private readonly ReservationService _reservations;

    public ReservationController(BookLibraryService bookLibrary, ReservationService reservations)
    {
        _bookLibrary = bookLibrary;
        _reservations = reservations;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int bookId)
    {
        var book = await _bookLibrary.GetBookById(bookId);

        return View(new ReservationCreateVm
        {
            BookId = book.Id,
            BookTitle = book.Title
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ReservationCreateVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            _reservations.Reserve(vm.BookId, vm.UserName);
            return RedirectToAction(nameof(Queue), new { bookId = vm.BookId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Queue(int bookId)
    {
        var book = await _bookLibrary.GetBookById(bookId);
        var queue = _reservations.GetQueue(bookId);

        return View(new ReservationQueueVm
        {
            BookId = book.Id,
            BookTitle = book.Title,
            Reservations = queue
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancel(int reservationId, int bookId)
    {
        _reservations.Cancel(reservationId);
        return RedirectToAction(nameof(Queue), new { bookId });
    }
}