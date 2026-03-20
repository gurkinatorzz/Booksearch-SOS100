
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;
using Microsoft.AspNetCore.Identity;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UsersController(UserDbContext context, PasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // Login endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return Unauthorized(new { message = "User has no password set." });
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(new LoginResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
                Message = "Login successful"
            });
        }

        // API Key validation endpoint
        [HttpGet("validate")]
        public IActionResult ValidateApiKey()
        {
            // If we reach here, the API key is valid (middleware passed)
            return Ok(new
            {
                Valid = true,
                Service = "UserService",
                Timestamp = DateTime.UtcNow,
                Message = "API Key is valid"
            });
        }
        
        // Health check endpoint (no API key required - handled by middleware)
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                Status = "Healthy",
                Service = "UserService",
                Timestamp = DateTime.UtcNow,
                UserCount = _context.Users.Count()
            });
        }

/* ------------------------------------------------------------------------- */
        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}