namespace SOA100_bookLibrary.DTOs.Books;

public record BookDetailsDTO (
    int Id,
    string Title,
    int Year,
    int AuthorId,
    string AuthorName,
    int CategoryId,
    string CategoryName
    );