using System.Security.Claims;
using Booksearch.Services;
using Booksearch.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            // Call the UserService login endpoint
            var loginResult = await _userApiService.LoginAsync(account.Username, account.Password);

            if (loginResult == null)
            {
                ViewBag.ErrorMessage = "Login failed: Invalid email or password";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            // Create claims for the authenticated user
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, loginResult.Name));
            identity.AddClaim(new Claim(ClaimTypes.Email, loginResult.Email));
            identity.AddClaim(new Claim("UserId", loginResult.Id.ToString()));
            
            if (loginResult.IsAdmin)
                identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            else
                identity.AddClaim(new Claim(ClaimTypes.Role, "Medarbetare"));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            // Redirect to return URL or Home
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(returnUrl);
        }
        catch (Exception)
        {
            ViewBag.ErrorMessage = "Login failed: Service unavailable";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }

    // Logout action
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}