using JustTip.Core.Models;

namespace JustTip.Core.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee> AddEmployeeAsync(int businessId, Employee employee);
        Employee? GetEmployee(int businessId, int employeeId);
        IQueryable<Employee> GetEmployees(int businessId);
        Task DeleteEmployeeAsync(int businessId, int employeeId);
        Task UpdateEmployeeAsync(Employee employee);
        Task SaveChangesAsync();
        Task<List<Employee>> GetEmployeesWithShiftsAsync(int businessId);
    }
} 