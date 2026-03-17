using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Booksearch.Controllers;
 
[Authorize]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> SignOutUser()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("Index", "Home");
    }
}