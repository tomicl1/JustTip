using JustTip.Core.Models;

namespace JustTip.Core.Interfaces
{
    public interface ITipRepository
    {
        Task<TipAllocation> AddTipAllocationAsync(TipAllocation tipAllocation);
        IQueryable<TipAllocation> GetTipAllocations(int employeeId);
    }
} 