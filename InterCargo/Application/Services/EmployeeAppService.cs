using InterCargo.BusinessLogic.Entities;
using InterCargo.BusinessLogic.Interfaces;
using InterCargo.Application.Interfaces;

namespace InterCargo.Application.Services;

public class EmployeeAppService : IEmployeeAppService
{
    private readonly IEmployeeService _employeeService;

    public EmployeeAppService(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public List<Employee> GetAllEmployees()
    {
        return _employeeService.GetAllEmployees();
    }

    public Employee GetEmployeeById(Guid id)
    {
        return _employeeService.GetEmployeeById(id);
    }

    public void AddEmployee(Employee employee)
    {
        _employeeService.AddEmployee(employee);
    }

    public void UpdateEmployee(Employee employee)
    {
        _employeeService.UpdateEmployee(employee);
    }

    public void DeleteEmployee(Guid id)
    {
        _employeeService.DeleteEmployee(id);
    }

    public async Task<Employee> GetEmployeeByUsername(string username)
    {
        return await _employeeService.GetEmployeeByUsername(username);
    }
}