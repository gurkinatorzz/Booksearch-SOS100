using Booksearch.Models.BookLibraryDtos;

namespace Booksearch.ViewModels;

public class LibraryAdmin
{
    public List<BookListDto> Books { get; set; } = new();
    
    //Create bok form
    public BookFormVM Form { get; set; } = new();
    
    public string? ApiError { get; set; }
}