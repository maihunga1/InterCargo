using InterCargo.BusinessLogic.Entities;

namespace InterCargo.DataAccess.Interfaces
{
    public interface IUserRepository
    {

        List<User> GetAllUsers();
        User GetUserById(Guid id);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(Guid id);
        Task<User> GetUserByUsername(string username);
    }
}
