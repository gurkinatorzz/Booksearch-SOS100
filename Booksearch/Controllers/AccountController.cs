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
        // Kolla användarnamn och lösenord
        bool accountValid = account.Username == "admin" && account.Password == "abc123";

        // Fel användarnamn eller lösenord
        if (accountValid == false)
        {
            ViewBag.ErrorMessage = "Login failed: Wrong username or password";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // Korrekt användarnamn och lösenord, logga in användaren
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, account.Username));
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        // Ifall vi inte har en returnUrl, gå till Home
        if (string.IsNullOrEmpty(returnUrl))
        {
            return RedirectToAction("Index", "Home");
        }

        // Gå tillbaka via returnUrl
        return Redirect(returnUrl);
    }
    
    // Logga ut 
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
