using System.Security.Claims;
using Booksearch.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Booksearch.Controllers;

public class AccountController : Controller
{
    public IActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM account, string returnUrl)
    {
        /* Hardcoded login credentials
        bool accountValid = account.Username == "admin" && account.Password == "abc123";

        // Check login credentials
        if (accountValid == false)
        {
            ViewBag.ErrorMessage = "Login failed: Wrong username or password";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        } */

        // Create claims for the authenticated user
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, account.Username));
        
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        // Redirect to return URL or Home
        if (string.IsNullOrEmpty(returnUrl))
        {
            return RedirectToAction("Index", "Home");
        }

        return Redirect(returnUrl);
    }
    
    // Logout action
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
