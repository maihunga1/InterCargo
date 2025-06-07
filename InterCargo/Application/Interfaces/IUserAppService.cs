using InterCargo.BusinessLogic.Entities;
using System.Collections.Generic;

namespace InterCargo.Application.Interfaces;

public interface IUserAppService
{
    Task<User> GetUserByUsername(string username);
    Task<User> GetUserByEmail(string email);
    Task<User> GetUserById(Guid id);
    List<User> GetAllUsers();
    void AddUser(User user);
    void UpdateUser(User user);
    void DeleteUser(Guid id);
    Task<bool> ValidateUserCredentials(string email, string password);
    Task<User?> GetUserById(string id);
    Task<List<User>> GetAllUsersAsync();
    Task<List<User>> SearchUsersAsync(string searchQuery);
}