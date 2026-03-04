namespace Booksearch.Models.BookLibraryDtos;

public class BookDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int Year { get; set; }
    
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = "";
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
}