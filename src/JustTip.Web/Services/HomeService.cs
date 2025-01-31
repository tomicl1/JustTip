using AutoMapper;
using JustTip.Core.Interfaces;
using JustTip.Web.Models.DTOs;

namespace JustTip.Web.Services
{
    public class HomeService : IHomeService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IMapper _mapper;

        public HomeService(IBusinessRepository businessRepository, IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _businessRepository = businessRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BusinessDto>> GetDashboardDataAsync()
        {
            var businesses = await _businessRepository.GetBusinessesWithEmployeesAndTipsAsync();

            return _mapper.Map<IEnumerable<BusinessDto>>(businesses);
        }

        public async Task<IEnumerable<TipAllocationSummaryDto>> GetRecentTipAllocationsAsync()
        {
            var businesses = await _businessRepository.GetBusinessesWithEmployeesAndTipsAsync();

            return businesses
                .SelectMany(b => b.Employees)
                .SelectMany(e => e.TipAllocations)
                .OrderByDescending(t => t.DistributionDate)
                .Take(10)
                .Select(t => new TipAllocationSummaryDto
                {
                    BusinessName = t.Employee.Business.Name,
                    EmployeeName = t.Employee.Name,
                    Amount = t.Amount,
                    DistributionDate = t.DistributionDate,
                    PeriodStart = t.PeriodStart,
                    PeriodEnd = t.PeriodEnd
                });
        }
    }
} 