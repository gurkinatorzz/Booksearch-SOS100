namespace SOA100_bookLibrary.Data;

//Denna klass är för att automatiskt lägga in databasobjekt, innehåll genererad av ChatGPT
//Detta görs så att det blir lättare för testning osv.
//-----kan tas bort senare när projektet är driftsatt och man lagt in egna föremål.
public static class DbSeeder
{
    public static void Seed(BookLibraryDbContext db)
    {
        // Om det redan finns data: gör inget
        if (db.Books.Any() || db.Authors.Any() || db.Categories.Any())
            return;

        var a1 = new Author { Name = "George Orwell" };
        var a2 = new Author { Name = "Jane Austen" };
        var a3 = new Author { Name = "J.R.R. Tolkien" };
        var a4 = new Author { Name = "Agatha Christie" };
        var a5 = new Author { Name = "Haruki Murakami" };

        var c1 = new Category { Name = "Classic" };
        var c2 = new Category { Name = "Fantasy" };
        var c3 = new Category { Name = "Crime" };
        var c4 = new Category { Name = "Romance" };
        var c5 = new Category { Name = "Fiction" };

        db.Authors.AddRange(a1, a2, a3, a4, a5);
        db.Categories.AddRange(c1, c2, c3, c4, c5);
        db.SaveChanges();

        db.Books.AddRange(
            new Book { Title = "1984", Year = 1949, AuthorId = a1.Id, CategoryId = c1.Id },
            new Book { Title = "Animal Farm", Year = 1945, AuthorId = a1.Id, CategoryId = c1.Id },

            new Book { Title = "Pride and Prejudice", Year = 1813, AuthorId = a2.Id, CategoryId = c4.Id },
            new Book { Title = "Sense and Sensibility", Year = 1811, AuthorId = a2.Id, CategoryId = c4.Id },

            new Book { Title = "The Hobbit", Year = 1937, AuthorId = a3.Id, CategoryId = c2.Id },
            new Book { Title = "The Fellowship of the Ring", Year = 1954, AuthorId = a3.Id, CategoryId = c2.Id },

            new Book { Title = "Murder on the Orient Express", Year = 1934, AuthorId = a4.Id, CategoryId = c3.Id },
            new Book { Title = "And Then There Were None", Year = 1939, AuthorId = a4.Id, CategoryId = c3.Id },

            new Book { Title = "Norwegian Wood", Year = 1987, AuthorId = a5.Id, CategoryId = c5.Id },
            new Book { Title = "Kafka on the Shore", Year = 2002, AuthorId = a5.Id, CategoryId = c5.Id }
        );

        db.SaveChanges();
    }
}
