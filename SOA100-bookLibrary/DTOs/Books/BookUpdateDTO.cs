namespace SOA100_bookLibrary.DTOs.Books;

public record BookUpdateDTO (
    string Title,
    int Year,
    int AuthorId,
    int CategoryId
);