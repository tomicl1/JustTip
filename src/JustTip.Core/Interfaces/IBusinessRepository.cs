using JustTip.Core.Models;

namespace JustTip.Core.Interfaces
{
    public interface IBusinessRepository
    {
        IQueryable<Business> GetAll();
        Business? GetById(int id, bool includeEmployees = false);
        Task<Business> AddAsync(Business business);
        Task UpdateAsync(Business business);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
        Task<Business?> GetByIdAsync(int id, bool includeEmployees = false);
        Task<Business?> GetBusinessWithEmployeesAndTipsAsync(int id);
        Task<List<Business>> GetBusinessesWithEmployeesAndTipsAsync();
    }
} 