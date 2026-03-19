using Booksearch.Services;

namespace Booksearch.ViewModels
{
    public class UserAdmin
    {
        public List<UserDto> Users { get; set; } = new();
        
        // Create user form
        public UserFormVM Form { get; set; } = new();
        
        public string? ApiError { get; set; }
        
        // For editing
        public UserDto? EditingUser { get; set; }
        public bool IsEditing { get; set; }
    }

    public class UserFormVM
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public bool IsAdmin { get; set; }
        public string? Password { get; set; } = ""; 
    }
}