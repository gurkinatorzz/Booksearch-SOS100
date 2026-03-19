using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Booksearch.Services;
using Booksearch.Models;
namespace Booksearch.Controllers;

[Authorize]
public class BookLoanController : Controller
{
    private readonly BookLibraryService _bookLibraryService;
    private readonly BookLoanApiService _bookLoanApiService;

    public BookLoanController(
        BookLibraryService bookLibraryService,
        BookLoanApiService bookLoanApiService)
    {
        _bookLibraryService = bookLibraryService;
        _bookLoanApiService = bookLoanApiService;
    }

    public async Task<IActionResult> Index()
    {
        var userLoans = await _bookLoanApiService.GetUserLoans(User.Identity!.Name!);
        return View(userLoans);
    }

    public IActionResult Create(int bookId, string bookTitle)
    {
        ViewBag.BookId = bookId;
        ViewBag.BookTitle = bookTitle;

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(BookLoan bookLoan)
    {
        bookLoan.BorrowerName = User.Identity!.Name!;

        var success = await _bookLoanApiService.CreateLoan(bookLoan);

        if (!success)
        {
            ViewBag.Error = "Du har redan lånat denna bok.";
            ViewBag.BookId = bookLoan.BookId;
            ViewBag.BookTitle = bookLoan.BookTitle;
            return View();
        }

        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> Return(int id)
    {
        Console.WriteLine($"Return körs för id: {id}");
        await _bookLoanApiService.ReturnLoan(id);
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Edit(int id)
    {
        var loans = await _bookLoanApiService.GetActiveLoans();
        var loan = loans.FirstOrDefault(x => x.Id == id);

        if (loan == null)
        {
            return RedirectToAction("Index");
        }

        return View(loan);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(BookLoan bookLoan)
    {
        await _bookLoanApiService.UpdateLoan(bookLoan);
        return RedirectToAction("Index");
    }
}