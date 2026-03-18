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
        var activeLoans = await _bookLoanApiService.GetActiveLoans();
        return View(activeLoans);
    }

    public IActionResult Create(int bookId, string bookTitle)
    {
        ViewBag.BookId = bookId;
        ViewBag.BookTitle = bookTitle;

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Booksearch.Models.BookLoan bookLoan)
    {
        bookLoan.BorrowerName = User.Identity!.Name!;
       
        
        await _bookLoanApiService.CreateLoan(bookLoan);
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