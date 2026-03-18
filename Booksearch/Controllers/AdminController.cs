using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Booksearch.Services;
using Booksearch.ViewModels;

namespace Booksearch.Controllers;
 
[Authorize]
public class AdminController : Controller
{
    private readonly UserApiService _userApiService;

    public AdminController(UserApiService userApiService)
    {
        _userApiService = userApiService;
    }

    public IActionResult Index()
    {
        return View();
    }

    // User Management - following your existing pattern
    public async Task<IActionResult> UserAdmin()
    {
        var viewModel = new UserAdmin();
        
        try
        {
            viewModel.Users = await _userApiService.GetAllUsersAsync();
        }
        catch (Exception ex)
        {
            viewModel.ApiError = $"Failed to load users: {ex.Message}";
        }
        
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(UserAdmin model)
    {
        if (!ModelState.IsValid)
        {
            // Reload users if validation fails
            model.Users = await _userApiService.GetAllUsersAsync();
            return View("UserAdmin", model);
        }

        try
        {
            var userDto = new UserDto
            {
                Name = model.Form.Name,
                Email = model.Form.Email,
                IsAdmin = model.Form.IsAdmin
                // No password handling - keep it null
            };

            var success = await _userApiService.CreateUserAsync(userDto);
            
            if (success)
            {
                TempData["SuccessMessage"] = "User created successfully!";
                return RedirectToAction(nameof(UserAdmin));
            }
            else
            {
                model.ApiError = "Failed to create user";
            }
        }
        catch (Exception ex)
        {
            model.ApiError = $"Error creating user: {ex.Message}";
        }

        // Reload users and return view
        model.Users = await _userApiService.GetAllUsersAsync();
        return View("UserAdmin", model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser(UserAdmin model)
    {
        if (model.EditingUser == null)
        {
            return RedirectToAction(nameof(UserAdmin));
        }

        try
        {
            var success = await _userApiService.UpdateUserAsync(model.EditingUser);
            
            if (success)
            {
                TempData["SuccessMessage"] = "User updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update user";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error updating user: {ex.Message}";
        }

        return RedirectToAction(nameof(UserAdmin));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var success = await _userApiService.DeleteUserAsync(id);
            
            if (success)
            {
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting user: {ex.Message}";
        }

        return RedirectToAction(nameof(UserAdmin));
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        try
        {
            var user = await _userApiService.GetUserAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction(nameof(UserAdmin));
            }

            var viewModel = new UserAdmin
            {
                Users = await _userApiService.GetAllUsersAsync(),
                EditingUser = user,
                IsEditing = true
            };

            return View("UserAdmin", viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading user: {ex.Message}";
            return RedirectToAction(nameof(UserAdmin));
        }
    }
}