using InterCargo.BusinessLogic.Entities;

namespace InterCargo.BusinessLogic.Interfaces;

public interface IUserService
{
    List<User> GetAllUsers();
    User GetUserById(Guid id);
    void AddUser(User user);
    void UpdateUser(User user);
    void DeleteUser(Guid id);
    Task<User> GetUserByUsername(string username);
}