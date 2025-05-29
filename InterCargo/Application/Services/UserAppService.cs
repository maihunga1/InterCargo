using InterCargo.Application.Interfaces;
using InterCargo.BusinessLogic.Entities;
using InterCargo.BusinessLogic.Interfaces;

namespace InterCargo.Application.Services;

public class UserAppService : IUserAppService
{
    private readonly IUserService _userService;

    public UserAppService(IUserService userService)
    {
        _userService = userService;
    }

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
