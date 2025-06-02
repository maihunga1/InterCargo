using InterCargo.Application.Interfaces;
using InterCargo.BusinessLogic.Entities;
using InterCargo.BusinessLogic.Interfaces;
using System.Collections.Generic;

namespace InterCargo.Application.Services;

public class UserAppService : IUserAppService
{
    private readonly IUserService _userService;

    public UserAppService(IUserService userService)
    {
        _userService = userService;
    }

    // Synchronous methods (required by interface)
    public List<User> GetAllUsers()
    {
        // Since IUserService doesn't have a GetAllUsers method, we'll return an empty list
        // In a real application, you'd implement this properly
        return new List<User>();
    }

    public User GetUserById(Guid id)
    {
        // Use the async method and block on it (not ideal, but required by interface)
        return _userService.GetUserByIdAsync(id).Result;
    }

    public void AddUser(User user)
    {
        _userService.CreateUserAsync(user).Wait();
    }

    public void UpdateUser(User user)
    {
        _userService.UpdateUserAsync(user).Wait();
    }

    public void DeleteUser(Guid id)
    {
        _userService.DeleteUserAsync(id).Wait();
    }

    public Task<User> GetUserByUsername(string username)
    {
        return _userService.GetUserByUsernameAsync(username);
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
}
