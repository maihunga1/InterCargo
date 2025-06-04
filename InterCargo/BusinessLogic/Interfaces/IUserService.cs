using InterCargo.BusinessLogic.Entities;

namespace InterCargo.BusinessLogic.Interfaces;

public interface IUserService
{
    Task<User> GetUserByIdAsync(Guid id);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetUserByUsernameAsync(string username);
    Task<bool> CreateUserAsync(User user);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(Guid id);
    Task<bool> ValidateUserCredentialsAsync(string email, string password);
}