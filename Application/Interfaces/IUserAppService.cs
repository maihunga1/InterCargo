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
}