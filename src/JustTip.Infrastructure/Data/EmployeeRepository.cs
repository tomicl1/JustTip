using JustTip.Core.Interfaces;
using JustTip.Core.Models;
using Microsoft.EntityFrameworkCore;
using JustTip.Core.Exceptions;

namespace JustTip.Infrastructure.Data
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly JustTipDbContext _context;

        public EmployeeRepository(JustTipDbContext context)
        {
            _context = context;
        }

        public async Task<Employee> AddEmployeeAsync(int businessId, Employee employee)
        {
            try
            {
                var business = _context.Businesses.Find(businessId);
                if (business == null) throw new ArgumentException("Business not found", nameof(businessId));
                employee.BusinessId = businessId;
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return employee;
            }
            catch (DbUpdateException ex)
            {
                throw new EmployeeOperationException("An error occurred while adding the employee.", ex);
            }
        }

        public Employee? GetEmployee(int businessId, int employeeId)
        {
            return _context.Employees
                .Include(e => e.TipAllocations)
                .Include(e => e.Shifts)
                .FirstOrDefault(e => e.BusinessId == businessId && e.Id == employeeId);
        }

        public IQueryable<Employee> GetEmployees(int businessId)
        {
            return _context.Employees
                .Include(e => e.Shifts)
                .Where(e => e.BusinessId == businessId);
        }

        public async Task DeleteEmployeeAsync(int businessId, int employeeId)
        {
            try
            {
                var employee = GetEmployee(businessId, employeeId);
                if (employee != null)
                {
                    _context.Employees.Remove(employee);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new EmployeeOperationException("An error occurred while deleting the employee.", ex);
            }
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            if (!_context.ChangeTracker.Entries<Employee>().Any(e => e.Entity.Id == employee.Id))
            {
                _context.Entry(employee).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Employee>> GetEmployeesWithShiftsAsync(int businessId)
        {
            return await _context.Employees
                .Where(e => e.BusinessId == businessId)
                .Include(e => e.Shifts)
                .ToListAsync();
        }
    }
}