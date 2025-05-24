using InterCargo.BusinessLogic.Entities;

namespace InterCargo.Pages.Employees.Models;

public class EmployeeViewModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string FamilyName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string EmployeeType { get; set; }
    public string Address { get; set; }
    public string EmployeeId { get; set; }
    public string Password { get; set; }
}

public static class EmployeeViewModelExtensions
{
    public static Employee ToEntity(this EmployeeViewModel model)
    {
        return new Employee
        {
            Id = model.Id,
            Username = model.Username,
            FirstName = model.FirstName,
            FamilyName = model.FamilyName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            EmployeeType = (EType)Enum.Parse(typeof(EType), model.EmployeeType),
            Address = model.Address,
            EmployeeId = model.EmployeeId,
            Password = model.Password
        };
    }
}