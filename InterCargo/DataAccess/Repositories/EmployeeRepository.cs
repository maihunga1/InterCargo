using InterCargo.BusinessLogic.Entities;
using InterCargo.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InterCargo.DataAccess.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void AddEmployee(Employee employee)
    {
        _context.Employees.Add(employee);
        _context.SaveChanges();
    }

    public void DeleteEmployee(Guid id)
    {
        var employee = _context.Employees.Find(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            _context.SaveChanges();
        }
    }

    public List<Employee> GetAllEmployees()
    {
        return _context.Employees.ToList();
    }

    public Employee GetEmployeeById(Guid id)
    {
        return _context.Employees.FirstOrDefault(e => e.Id == id);
    }

    public void UpdateEmployee(Employee employee)
    {
        _context.Employees.Update(employee);
        _context.SaveChanges();
    }

    public async Task<Employee> GetEmployeeByUsername(string username)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.Username == username);
    }

    public async Task<Employee> GetEmployeeByEmail(string email)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
    }
}