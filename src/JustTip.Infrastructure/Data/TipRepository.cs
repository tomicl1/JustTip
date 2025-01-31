using JustTip.Core.Interfaces;
using JustTip.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace JustTip.Infrastructure.Data
{
    public class TipRepository : ITipRepository
    {
        private readonly JustTipDbContext _context;

        public TipRepository(JustTipDbContext context)
        {
            _context = context;
        }

        public async Task<TipAllocation> AddTipAllocationAsync(TipAllocation tipAllocation)
        {
            _context.TipAllocations.Add(tipAllocation);
            await _context.SaveChangesAsync();
            return tipAllocation;
        }

        public IQueryable<TipAllocation> GetTipAllocations(int employeeId)
        {
            return _context.TipAllocations
                .AsNoTracking()
                .Where(t => t.EmployeeId == employeeId)
                .OrderByDescending(t => t.DistributionDate);
        }
    }
} 