using JustTip.Web.Models.DTOs;

namespace JustTip.Web.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeDto?> GetEmployeeDtoByIdAsync(int businessId, int employeeId);
        Task AddEmployeeAsync(int businessId, EmployeeDto employeeDto);
        Task UpdateEmployeeAsync(int businessId, EmployeeDto employeeDto);
        Task DeleteEmployeeAsync(int businessId, int employeeId);
    }
} 