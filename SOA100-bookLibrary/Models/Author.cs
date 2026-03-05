namespace SOA100_bookLibrary;

public class Author
{
    //primär-nyckel till databasen, auto-increment
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    
    //en författare kan ha flera böcker
    public List<Book> Books { get; set; } = new();
}