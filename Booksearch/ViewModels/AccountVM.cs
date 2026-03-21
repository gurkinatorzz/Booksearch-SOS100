using System.ComponentModel.DataAnnotations;

namespace Booksearch.ViewModels
{
    public class AccountVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }

    public class UpdateAccountVM
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Namn är obligatoriskt")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "E-post är obligatoriskt")]
        [EmailAddress(ErrorMessage = "Ange en giltig e-postadress")]
        public string Email { get; set; } = string.Empty;
    }

    public class ChangePasswordVM
    {
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "Nuvarande lösenord är obligatoriskt")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Nytt lösenord är obligatoriskt")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Lösenordet måste vara minst 6 tecken")]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Bekräfta nytt lösenord")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}