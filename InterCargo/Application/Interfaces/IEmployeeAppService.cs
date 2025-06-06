using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InterCargo.BusinessLogic.Entities;

namespace InterCargo.Application.Interfaces
{
    public interface IEmployeeAppService
    {
        List<Employee> GetAllEmployees();
        Employee GetEmployeeById(Guid id);
        void AddEmployee(Employee employee);
        void UpdateEmployee(Employee employee);
        void DeleteEmployee(Guid id);
        Task<Employee> GetEmployeeByUsername(string username);
        Task<Employee> GetEmployeeByEmail(string email);
    }
}
