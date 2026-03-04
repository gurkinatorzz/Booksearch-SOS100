namespace Booksearch.Models.BookLibraryDtos;

public record BookListDto(
    int Id,
    string Title,
    int Year,
    string Author,
    string Category 
    );