using BookLoanService.Data;
using BookLoanService.Models;
using Microsoft.AspNetCore.Mvc;
namespace BookLoanService.Controllers;

[ApiController]
[Route("api/[controller]")]

public class BookLoanController : ControllerBase
{
    
    private readonly BookLoanServiceDbContext _DbContext;
    
    public BookLoanController(BookLoanServiceDbContext dbContext)
    {
        _DbContext = dbContext;
    }

    [HttpGet]
        
    public BookLoan[] GetBookLoans()
    {
        
        BookLoan[]  bookLoans = _DbContext.BookLoans.ToArray();
        return bookLoans;
    }

    [HttpPost]
    
        public void PostBookLoan(BookLoan bookLoan)
        {
           _DbContext.BookLoans.Add(bookLoan);
           _DbContext.SaveChanges();
           
        }
        
    //Hämtar böcker som inte är återlämnade
    [HttpGet("active")]
    public BookLoan[] GetActiveLoans()
    {
        return _DbContext.BookLoans
            .Where(x => x.ReturnedDate == null)
            .ToArray();
    }
    [HttpPut("return/{id}")]
    public void ReturnBookLoan(int id)
    {
        var bookLoan = _DbContext.BookLoans.FirstOrDefault(x => x.Id == id);

        if (bookLoan != null)
        {
            bookLoan.ReturnedDate = DateTime.Now;
            _DbContext.SaveChanges();
        }
    }
    [HttpPut("{id}")]
    public void UpdateBookLoan(int id, BookLoan updatedLoan)
    {
        var bookLoan = _DbContext.BookLoans.FirstOrDefault(x => x.Id == id);

        if (bookLoan != null)
        {
            bookLoan.LoanDate = updatedLoan.LoanDate;
            bookLoan.DueDate = updatedLoan.DueDate;

            _DbContext.SaveChanges();
        }
    }
    }
    
    
    