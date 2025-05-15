using InterCargo.BusinessLogic.Entities;
using InterCargo.BusinessLogic.Interfaces;
using InterCargo.Application.Interfaces;

namespace InterCargo.Application.Services;

public class UserAppService : IUserAppService
{
    private readonly IUserService _userService;

    public UserAppService(IUserService userService)
    {
        _userService = userService;
    }

    public List<User> GetAllUsers()
    {
        return _userService.GetAllUsers();
    }

    public User GetUserById(Guid id)
    {
        return _userService.GetUserById(id);
    }

    public void AddUser(User user)
    {
        _userService.AddUser(user);
    }

    public void UpdateUser(User user)
    {
        _userService.UpdateUser(user);
    }

    public void DeleteUser(Guid id)
    {
        _userService.DeleteUser(id);
    }

    public async Task<User> GetUserByUsername(string username)
    {
        return await _userService.GetUserByUsername(username);
    }
}
