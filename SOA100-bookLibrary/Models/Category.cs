namespace SOA100_bookLibrary;

public class Category
{

        //primär-nyckel till databasen, auto-increment
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    
        //en kategori kan ha flera böcker
        public List<Book> Books { get; set; } = new();
}