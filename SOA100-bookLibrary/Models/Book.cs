using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SOA100_bookLibrary;

public class Book
{
    
    //primär-nyckel till databasen, auto-increment
    public int Id { get; set; }
    
    //Attribut för boken till databasen
    public string Title { get; set; } = null!;
    public int Year { get; set; }
    
    //foreign keys
    public int AuthorId { get; set; }
    public int CategoryId { get; set; }
    
    
    //navigation properties
    public Author Author { get; set; } = null!;
    public Category Category { get; set; } = null!;
}