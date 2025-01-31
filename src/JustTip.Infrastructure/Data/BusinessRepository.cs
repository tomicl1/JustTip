using JustTip.Core.Interfaces;
using JustTip.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace JustTip.Infrastructure.Data
{
    public class BusinessRepository : IBusinessRepository
    {
        private readonly JustTipDbContext _context;

        public BusinessRepository(JustTipDbContext context)
        {
            _context = context;
        }

        public IQueryable<Business> GetAll()
        {
            return _context.Businesses
                .Include(b => b.Employees)
                .AsNoTracking();
        }

        public Business? GetById(int id, bool includeEmployees = false)
        {
            IQueryable<Business> query = _context.Businesses;
            
            if (includeEmployees)
            {
                query = query.Include(b => b.Employees)
                            .ThenInclude(e => e.Shifts);
            }

            return query.FirstOrDefault(b => b.Id == id);
        }

        public async Task<Business?> GetByIdAsync(int id, bool includeEmployees = false)
        {
            IQueryable<Business> query = _context.Businesses;
            
            if (includeEmployees)
            {
                query = query.Include(b => b.Employees)
                            .ThenInclude(e => e.Shifts);
            }

            return await query.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Business> AddAsync(Business business)
        {
            try
            {
                _context.Businesses.Add(business);
                await SaveChangesAsync();
                return business;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while adding the business.", ex);
            }
        }

        public async Task UpdateAsync(Business business)
        {
            try
            {
                if (!_context.ChangeTracker.Entries<Business>().Any(e => e.Entity.Id == business.Id))
                {
                    _context.Entry(business).State = EntityState.Modified;
                }
                await SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while updating the business.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var business = await _context.Businesses.FindAsync(id);
                if (business != null)
                {
                    _context.Businesses.Remove(business);
                    await SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while deleting the business.", ex);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Business?> GetBusinessWithEmployeesAndTipsAsync(int id)
        {
            return await _context.Businesses
                .Include(b => b.Employees)
                .ThenInclude(e => e.TipAllocations.Where(t => t.DistributionDate > DateTime.Now.AddMonths(-1)))
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Business>> GetBusinessesWithEmployeesAndTipsAsync()
        {
            return await _context.Businesses
                .Include(b => b.Employees)
                .ThenInclude(e => e.TipAllocations)
                .ToListAsync();
        }
    }
} 