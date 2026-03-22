using System.Security.Claims;
using Booksearch.Services;
using Booksearch.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    public async Task<IActionResult> MyAccount()
    {
        try
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _userApiService.GetUserAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var accountVM = new AccountVM
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsAdmin = user.IsAdmin
            };

            return View(accountVM);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Det gick inte att hämta kontoinformation: " + ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateAccount(UpdateAccountVM model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        try
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId) || userId != model.Id)
            {
                return Json(new { success = false, message = "Otillåten åtgärd" });
            }

            var userDto = new UserDto
            {
                Id = model.Id,
                Name = model.Name,
                Email = model.Email
            };

            var success = await _userApiService.UpdateUserAsync(userDto);

            if (success)
            {
                // Update the claims if email or name changed
                await UpdateUserClaimsAsync(model.Name, model.Email);
                return Json(new { success = true, message = "Kontoinformation uppdaterad" });
            }
            else
            {
                return Json(new { success = false, message = "Det gick inte att uppdatera kontoinformation" });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Fel: " + ex.Message });
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        try
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId) || userId != model.UserId)
            {
                return Json(new { success = false, message = "Otillåten åtgärd" });
            }

            var success = await _userApiService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

            if (success)
            {
                return Json(new { success = true, message = "Lösenord ändrat" });
            }
            else
            {
                return Json(new { success = false, message = "Det gick inte att ändra lösenord. Kontrollera att nuvarande lösenord är korrekt." });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Fel: " + ex.Message });
        }
    }

    private async Task UpdateUserClaimsAsync(string name, string email)
    {
        // Get current claims
        var identity = (ClaimsIdentity)User.Identity!;
        
        // Remove old name and email claims
        var nameClaim = identity.FindFirst(ClaimTypes.Name);
        var emailClaim = identity.FindFirst(ClaimTypes.Email);
        
        if (nameClaim != null) identity.RemoveClaim(nameClaim);
        if (emailClaim != null) identity.RemoveClaim(emailClaim);
        
        // Add new claims
        identity.AddClaim(new Claim(ClaimTypes.Name, name));
        identity.AddClaim(new Claim(ClaimTypes.Email, email));
        
        // Re-sign in with updated claims
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }
}