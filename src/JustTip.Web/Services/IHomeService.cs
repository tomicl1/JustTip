using JustTip.Web.Models.DTOs;

namespace JustTip.Web.Services
{
    public interface IHomeService
    {
        Task<IEnumerable<BusinessDto>> GetDashboardDataAsync();
        Task<IEnumerable<TipAllocationSummaryDto>> GetRecentTipAllocationsAsync();
    }
} 