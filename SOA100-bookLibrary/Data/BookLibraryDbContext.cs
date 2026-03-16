using Microsoft.EntityFrameworkCore;

namespace SOA100_bookLibrary.Data;

public class BookLibraryDbContext : DbContext
{
    public BookLibraryDbContext(DbContextOptions<BookLibraryDbContext> options)
        : base(options) { }

    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
}