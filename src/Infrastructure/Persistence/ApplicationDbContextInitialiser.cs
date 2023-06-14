﻿using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;
public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
    
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        var administratorRole = new IdentityRole("Administrator");
        var userRole = new IdentityRole("User");

        // Default roles
        var roles = new List<IdentityRole>
        {
            administratorRole,
            userRole
        };

        foreach (var role in roles)
        {
            if (_roleManager.Roles.All(r => r.Name != role.Name))
            {
                await _roleManager.CreateAsync(role);
            }
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new[] { "Administrator" });
            }
        }

        var user = new ApplicationUser { UserName = "user@localhost", Email = "user@localhost" };

        if (_userManager.Users.All(u => u.UserName != user.UserName))
        {
            await _userManager.CreateAsync(user, "User1!");
            if (!string.IsNullOrWhiteSpace(userRole.Name))
            {
                await _userManager.AddToRolesAsync(user, new[] { "User" });
            }
        }

        // Default data
        // Seed, if necessary
        if (!_context.TodoLists.Any())
        {
            _context.TodoLists.Add(new TodoList
            {
                Title = "Todo List",
                Items =
            {
                new TodoItem { Title = "Make a todo list 📃" },
                new TodoItem { Title = "Check off the first item ✅" },
                new TodoItem { Title = "Realise you've already done two things on the list! 🤯"},
                new TodoItem { Title = "Reward yourself with a nice, long nap 🏆" },
            }
            });

            await _context.SaveChangesAsync();
        }
    }

}
