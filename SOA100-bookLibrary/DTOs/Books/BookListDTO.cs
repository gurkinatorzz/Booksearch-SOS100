namespace SOA100_bookLibrary.DTOs;

public record BookListDTO (
    int Id,
    string Title,
    int Year,
    string Author,
    string Category 
    );
