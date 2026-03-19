using Microsoft.AspNetCore.Identity;
using UserService.Models;

namespace UserService.Data;

public static class UserDbSeeder
{
    public static void Seed(UserDbContext context, PasswordHasher<User> passwordHasher)
    {
        // Only seed if database is empty
        if (context.Users.Any())
            return;

        var users = new List<User>
        {
            new User
            {
                Name = "System Administrator",
                Email = "admin@booksearch.com",
                IsAdmin = true,
                PasswordHash = passwordHasher.HashPassword(null, "Admin123!")
            },
            new User
            {
                Name = "John Doe",
                Email = "john.doe@student.hv.se",
                IsAdmin = false,
                PasswordHash = passwordHasher.HashPassword(null, "Student123!")
            },
            new User
            {
                Name = "Emma Andersson",
                Email = "emma.andersson@student.hv.se",
                IsAdmin = false,
                PasswordHash = passwordHasher.HashPassword(null, "Student123!")
            },
            new User
            {
                Name = "Lars Nilsson",
                Email = "lars.nilsson@staff.hv.se",
                IsAdmin = true,
                PasswordHash = passwordHasher.HashPassword(null, "Staff123!")
            },
            new User
            {
                Name = "Sofia Johansson",
                Email = "sofia.johansson@student.hv.se",
                IsAdmin = false,
                PasswordHash = passwordHasher.HashPassword(null, "Student123!")
            },
            new User
            {
                Name = "Marcus Berg",
                Email = "marcus.berg@student.hv.se",
                IsAdmin = false,
                PasswordHash = passwordHasher.HashPassword(null, "Student123!")
            },
            new User
            {
                Name = "Anna Lindgren",
                Email = "anna.lindgren@staff.hv.se",
                IsAdmin = true,
                PasswordHash = passwordHasher.HashPassword(null, "Staff123!")
            },
            new User
            {
                Name = "Test User",
                Email = "test@example.com",
                IsAdmin = false,
                PasswordHash = passwordHasher.HashPassword(null, "Test123!")
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();
    }
}