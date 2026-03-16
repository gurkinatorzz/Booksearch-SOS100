using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace Admin.Controllers;
 
[Authorize]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}