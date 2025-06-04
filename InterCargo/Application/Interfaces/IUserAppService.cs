using InterCargo.BusinessLogic.Entities;

namespace InterCargo.Application.Interfaces;

public interface IUserAppService
{
    List<User> GetAllUsers();
    User GetUserById(Guid id);
    void AddUser(User user);
    void UpdateUser(User user);
    void DeleteUser(Guid id);
    Task<User> GetUserByUsername(string username);
    Task<User> GetUserByIdAsync(Guid id);
    Task<User> GetUserByEmailAsync(string email);
    Task<bool> CreateUserAsync(User user);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(Guid id);
    Task<bool> ValidateUserCredentialsAsync(string email, string password);
}