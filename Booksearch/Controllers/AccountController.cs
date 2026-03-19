using System.Security.Claims;
using Booksearch.Services;
using Booksearch.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Booksearch.Controllers;

public class AccountController : Controller
{
    private readonly UserApiService _userApiService;

    public AccountController(UserApiService userApiService)
    {
        _userApiService = userApiService;
    }

    public IActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM account, string returnUrl)
    {
        try
        {
            // Get all users from UserService
            var users = await _userApiService.GetAllUsersAsync();
            
            // Find user by email (assuming username is email)
            var user = users.FirstOrDefault(u => u.Email.Equals(account.Username, StringComparison.OrdinalIgnoreCase));
            
            if (user == null)
            {
                ViewBag.ErrorMessage = "Login failed: User not found";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            // TODO: You'll need to add a password verification endpoint to your UserService
            // For now, this is still a limitation since UserService doesn't expose password verification
            
            // Create claims for the authenticated user
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            
            if (user.IsAdmin)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            }

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(returnUrl);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"Login failed: {ex.Message}";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
    
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
