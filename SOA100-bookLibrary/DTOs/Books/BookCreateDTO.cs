namespace SOA100_bookLibrary.DTOs.Books;

public record BookCreateDTO (
    string Title,
    int Year,
    int AuthorId,
    int CategoryId
    );
