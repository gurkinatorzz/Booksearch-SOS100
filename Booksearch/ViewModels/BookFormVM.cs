using Booksearch.Models.BookLibraryDtos;

namespace Booksearch.ViewModels;

public class BookFormVM
{
    public int?  Id { get; set; } //null vid create

    public string Title { get; set; } = "";
    public int Year { get; set; }
    
    public int AuthorId { get; set; }
    public int CategoryId { get; set; }

    public List<AuthorListDto> Authors { get; set; } = new();
    public List<CategoryListDto> Categories { get; set; } = new();
    
    //om API inte fungerar
    public string? ApiError { get; set; }
    
    //skpar nya författare och kategorier om dessa inte finns när ny bok skapas
    public string? NewAuthorName { get; set; }
    public string? NewCategoryName { get; set; }

}