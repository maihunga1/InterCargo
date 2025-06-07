using InterCargo.Application.Interfaces;
using InterCargo.BusinessLogic.Entities;
using InterCargo.BusinessLogic.Interfaces;
using System.Collections.Generic;
using InterCargo.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace InterCargo.Application.Services;

public class UserAppService : IUserAppService
{
    private readonly IUserService _userService;
    private readonly ApplicationDbContext _context;

    public UserAppService(IUserService userService, ApplicationDbContext context)
    {
        _userService = userService;
        _context = context;
    }

    // Synchronous methods (required by interface)
    public List<User> GetAllUsers()
    {
        return _context.Users.ToList();
    }

    public async Task<User> GetUserById(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public void AddUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void UpdateUser(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void DeleteUser(Guid id)
    {
        var user = _context.Users.Find(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }

    public async Task<User> GetUserByUsername(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    // Asynchronous methods (already implemented)
    public async Task<User> GetUserByIdAsync(Guid id)
    {
        return await _userService.GetUserByIdAsync(id);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _userService.GetUserByEmailAsync(email);
    }

    public async Task<bool> CreateUserAsync(User user)
    {
        return await _userService.CreateUserAsync(user);
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        return await _userService.UpdateUserAsync(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        return await _userService.DeleteUserAsync(id);
    }

    public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
    {
        return await _userService.ValidateUserCredentialsAsync(email, password);
    }

    public async Task<bool> ValidateUserCredentials(string email, string password)
    {
        var user = await GetUserByEmail(email) ?? await GetUserByUsername(email);
        if (user == null) return false;

        // Compare hashed passwords
        using (var sha256 = SHA256.Create())
        {
            var hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return user.Password == hashedPassword;
        }
    }

    public async Task<List<User>> SearchUsersAsync(string searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
            return new List<User>();

        var allUsers = await _userService.GetAllUsersAsync();
        return allUsers.Where(u => 
            (u.FirstName + " " + u.FamilyName).Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
            u.Email.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
        ).ToList();
    }

    public async Task<User?> GetUserById(string id)
    {
        if (Guid.TryParse(id, out var guid))
        {
            return await _userService.GetUserByIdAsync(guid);
        }
        return null;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _userService.GetAllUsersAsync();
    }
}
