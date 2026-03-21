using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Booksearch.Services;
using Booksearch.ViewModels;
using System.Text.Json;

namespace Booksearch.Controllers;

[Authorize(Roles = "Admin")]
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

    public async Task<IActionResult> UserAdmin()
    {
        var model = new UserAdmin();
        
        try
        {
            model.Users = await _userApiService.GetAllUsersAsync();
        }
        catch (Exception ex)
        {
            model.ApiError = $"Failed to load users: {ex.Message}";
            model.Users = new List<UserDto>();
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(UserAdmin model)
    {
        try
        {
            // Convert UserFormVM to UserDto
            var userDto = new UserDto
            {
                Name = model.Form.Name,
                Email = model.Form.Email,
                IsAdmin = model.Form.IsAdmin
            };

            var success = await _userApiService.CreateUserAsync(userDto);
            if (success)
            {
                TempData["SuccessMessage"] = "User created successfully!";
                
                // ✅ If password was provided, set it
                if (!string.IsNullOrWhiteSpace(model.Form.Password))
                {
                    // Get the created user to get their ID
                    var users = await _userApiService.GetAllUsersAsync();
                    var createdUser = users.FirstOrDefault(u => u.Email == model.Form.Email);
                    
                    if (createdUser != null)
                    {
                        var passwordSuccess = await _userApiService.SetUserPasswordAsync(createdUser.Id, model.Form.Password);
                        if (passwordSuccess)
                        {
                            TempData["SuccessMessage"] = "User created with password successfully!";
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "User created but password setting failed.";
                        }
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create user.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error creating user: {ex.Message}";
        }

        return RedirectToAction("UserAdmin");
    }

    [HttpPost]
    public async Task<IActionResult> SetUserPassword(int userId, string password)
    {
        try
        {
            var success = await _userApiService.SetUserPasswordAsync(userId, password);
            if (success)
            {
                TempData["SuccessMessage"] = "Password set successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to set password.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error setting password: {ex.Message}";
        }

        return RedirectToAction("UserAdmin");
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
                TempData["ErrorMessage"] = "Failed to delete user.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting user: {ex.Message}";
        }

        return RedirectToAction("UserAdmin");
    }

    public async Task<IActionResult> EditUser(int id)
    {
        var model = new UserAdmin();
        
        try
        {
            model.Users = await _userApiService.GetAllUsersAsync();
            model.EditingUser = await _userApiService.GetUserAsync(id);
            model.IsEditing = true;

            if (model.EditingUser == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("UserAdmin");
            }
        }
        catch (Exception ex)
        {
            model.ApiError = $"Failed to load user: {ex.Message}";
            model.Users = new List<UserDto>();
        }

        return View("UserAdmin", model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser(UserAdmin model)
    {
        try
        {
            var success = await _userApiService.UpdateUserAsync(model.EditingUser);
            if (success)
            {
                TempData["SuccessMessage"] = "User updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update user.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error updating user: {ex.Message}";
        }

        return RedirectToAction("UserAdmin");
    }
}