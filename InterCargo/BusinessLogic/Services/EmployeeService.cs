using InterCargo.BusinessLogic.Entities;
using InterCargo.BusinessLogic.Interfaces;
using InterCargo.DataAccess.Interfaces;

namespace InterCargo.BusinessLogic.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public List<Employee> GetAllEmployees()
    {
        return _employeeRepository.GetAllEmployees();
    }

    public Employee GetEmployeeById(Guid id)
    {
        return _employeeRepository.GetEmployeeById(id);
    }

    public void AddEmployee(Employee employee)
    {
        _employeeRepository.AddEmployee(employee);
    }

    public void UpdateEmployee(Employee employee)
    {
        _employeeRepository.UpdateEmployee(employee);
    }

    public void DeleteEmployee(Guid id)
    {
        _employeeRepository.DeleteEmployee(id);
    }

    public async Task<Employee> GetEmployeeByUsername(string username)
    {
        return await _employeeRepository.GetEmployeeByUsername(username);
    }
}

