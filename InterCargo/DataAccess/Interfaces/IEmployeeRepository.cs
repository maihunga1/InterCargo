using InterCargo.BusinessLogic.Entities;

namespace InterCargo.DataAccess.Interfaces
{
    public interface IEmployeeRepository
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
