using Booksearch.Models.BookLibraryDtos;

namespace Booksearch.ViewModels;

public class LibraryAdmin
{
    public List<BookListDto> Books { get; set; } = new();
    public List<AuthorListDto> Authors { get; set; } = new();
    public List<CategoryListDto> Categories { get; set; } = new();

    //Create bok form
    public BookFormVM Form { get; set; } = new();
    
    public AuthorUpdateDto AuthorForm { get; set; } = new();
    public CategoryUpdateDto CategoryForm { get; set; } = new();
    
    public string? ApiError { get; set; }
}