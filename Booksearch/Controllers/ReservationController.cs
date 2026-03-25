using Booksearch.Services;
using Booksearch.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booksearch.Controllers;

[Authorize]
public class ReservationController : Controller
{
    private readonly BookLibraryService _bookLibrary;
    private readonly ReservationService _reservations;
    private readonly BookLoanApiService _bookLoanApiService;

    public ReservationController(
        BookLibraryService bookLibrary,
        ReservationService reservations,
        BookLoanApiService bookLoanApiService)
    {
        _bookLibrary = bookLibrary;
        _reservations = reservations;
        _bookLoanApiService = bookLoanApiService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int bookId)
    {
        var book = await _bookLibrary.GetBookById(bookId);

        var activeLoans = await _bookLoanApiService.GetActiveLoans();
        var isLoaned = activeLoans.Any(x => x.BookId == bookId);

        Console.WriteLine("------ DEBUG GET ------");
        Console.WriteLine("BookId: " + bookId);
        Console.WriteLine("Loans count: " + activeLoans.Count());
        Console.WriteLine("IsLoaned: " + isLoaned);
        Console.WriteLine("-----------------------");

        if (!isLoaned)
        {
            TempData["ReservationError"] = "Det går inte att reservera eftersom boken inte är utlånad.";
            return RedirectToAction(nameof(MyReservations));
        }

        return View(new ReservationCreateVm
        {
            BookId = book.Id,
            BookTitle = book.Title
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReservationCreateVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var activeLoans = await _bookLoanApiService.GetActiveLoans();
            var isLoaned = activeLoans.Any(x => x.BookId == vm.BookId);

            Console.WriteLine("------ DEBUG POST ------");
            Console.WriteLine("BookId: " + vm.BookId);
            Console.WriteLine("Loans count: " + activeLoans.Count());
            Console.WriteLine("IsLoaned: " + isLoaned);
            Console.WriteLine("User: " + User.Identity!.Name);
            Console.WriteLine("------------------------");

            if (!isLoaned)
            {
                ModelState.AddModelError("", "Det går inte att reservera eftersom boken inte är utlånad.");
                return View(vm);
            }

            vm.UserName = User.Identity!.Name!;
            await _reservations.Reserve(vm.BookId, vm.UserName);

            return RedirectToAction(nameof(MyReservations));
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
        var queue = await _reservations.GetQueue(bookId);

        return View(new ReservationQueueVm
        {
            BookId = book.Id,
            BookTitle = book.Title,
            Reservations = queue
        });
    }

    [HttpGet]
    public async Task<IActionResult> MyReservations()
    {
        var userName = User.Identity!.Name!;
        var reservations = await _reservations.GetMyReservations(userName);

        foreach (var r in reservations)
        {
            var book = await _bookLibrary.GetBookById(r.BookId);
            r.Title = book.Title;

            var queue = await _reservations.GetQueue(r.BookId);
            var place = queue.ToList().FindIndex(x => x.Id == r.Id) + 1;
            r.QueuePosition = place;
        }

        return View("Queue", new ReservationQueueVm
        {
            BookId = 0,
            BookTitle = "Mina reservationer",
            Reservations = reservations
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int reservationId, int bookId)
    {
        IEnumerable<Booksearch.Models.BookReservation> queue;

        if (bookId == 0)
        {
            var userName = User.Identity!.Name!;
            queue = await _reservations.GetMyReservations(userName);
        }
        else
        {
            queue = await _reservations.GetQueue(bookId);
        }

        var reservation = queue.FirstOrDefault(r => r.Id == reservationId);

        if (reservation == null)
            return RedirectToAction(nameof(MyReservations));

        if (reservation.UserName != User.Identity!.Name)
            return Forbid();

        await _reservations.Cancel(reservationId);
        return RedirectToAction(nameof(MyReservations));
    }
}