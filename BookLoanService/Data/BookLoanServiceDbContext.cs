using BookLoanService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLoanService.Data;

public class BookLoanServiceDbContext : DbContext
{
    public BookLoanServiceDbContext(DbContextOptions<BookLoanServiceDbContext> options)
        : base(options) {  }
    
    public DbSet<BookLoan> BookLoans { get; set; }
}