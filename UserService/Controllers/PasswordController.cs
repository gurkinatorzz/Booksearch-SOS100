using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public PasswordController(UserDbContext context, PasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public class SetPasswordRequest
        {
            public string Password { get; set; } = string.Empty;
        }

        public class ChangePasswordRequest
        {
            public string CurrentPassword { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
        }

        // POST: api/password/5/set
        [HttpPost("{id}/set")]
        public async Task<IActionResult> SetPassword(int id, [FromBody] SetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Password is required.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/password/5/change
        [HttpPost("{id}/change")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("CurrentPassword and NewPassword are required.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.CurrentPassword);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return BadRequest("Current password is incorrect.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}