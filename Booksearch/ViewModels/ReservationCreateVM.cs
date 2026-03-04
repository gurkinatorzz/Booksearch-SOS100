using System.ComponentModel.DataAnnotations;

namespace Booksearch.ViewModels;

public class ReservationCreateVm
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = "";

    [Required(ErrorMessage = "Namn måste fyllas i")]
    public string UserName { get; set; } = "";
}