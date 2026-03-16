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
    }
    
    
    