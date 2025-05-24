using InterCargo.BusinessLogic.Entities;

namespace InterCargo.Pages.Users.Models;

public class UserViewModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string FamilyName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string CompanyName { get; set; }
    public string Address { get; set; }
}

public static class UserViewModelExtensions
{
    public static User ToEntity(this UserViewModel model)
    {
        return new User
        {
            Username = model.UserName,
            Email = model.Email,
            Password = model.Password,
            FirstName = model.FirstName,
            FamilyName = model.FamilyName,
            PhoneNumber = model.PhoneNumber
        };
    }
}