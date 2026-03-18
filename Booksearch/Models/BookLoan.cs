namespace Booksearch.Models;

public class BookLoan
{
    public int Id { get; set; }

    public int BookId { get; set; }

    public string BookTitle { get; set; } = string.Empty;

    public string BorrowerName { get; set; } = string.Empty;

    public DateTime LoanDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnedDate { get; set; }
    
    public bool IsComplete  { get; set; }
}