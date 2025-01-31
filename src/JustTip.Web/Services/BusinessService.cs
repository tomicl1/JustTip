using AutoMapper;
using JustTip.Core.Exceptions;
using JustTip.Core.Interfaces;
using JustTip.Web.Models.DTOs;
using JustTip.Web.Models.ViewModels;

namespace JustTip.Web.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public BusinessService(
            IBusinessRepository businessRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _businessRepository = businessRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<BusinessDto?> GetBusinessDtoByIdAsync(int id)
        {
            var business = await _businessRepository.GetBusinessWithEmployeesAndTipsAsync(id);

            if (business == null)
            {
                throw new BusinessNotFoundException(id);
            }

            var businessDto = _mapper.Map<BusinessDto>(business);
            businessDto.EmployeeCount = business.Employees.Count;
            businessDto.Employees = business.Employees.Select(e =>
            {
                var dto = _mapper.Map<EmployeeDto>(e);
                dto.LastTipAmount = e.TipAllocations
                    .OrderByDescending(t => t.DistributionDate)
                    .FirstOrDefault()?.Amount;
                return dto;
            }).ToList();

            return businessDto;
        }

        public async Task<(TipDistributionViewModel ViewModel, string ErrorMessage)> CalculateTipsAsync(TipDistributionViewModel viewModel)
        {
            try
            {
                var business = await _businessRepository.GetByIdAsync(viewModel.BusinessId);
                if (business == null)
                {
                    throw new BusinessNotFoundException(viewModel.BusinessId);
                }

                viewModel.BusinessName = business.Name;

                var employees = await _employeeRepository.GetEmployeesWithShiftsAsync(viewModel.BusinessId);

                var workedDaysDict = employees.ToDictionary(e => e.Id, e => e.Shifts
                    .Where(s =>
                    {
                        var shiftDate = new DateTime(s.Year, s.Month, 1);
                        var shiftEndDate = new DateTime(s.Year, s.Month, DateTime.DaysInMonth(s.Year, s.Month));
                        return shiftDate <= viewModel.PeriodEnd && shiftEndDate >= viewModel.PeriodStart;
                    })
                    .SelectMany(s => s.GetWorkedDays()
                        .Select(day => new DateTime(s.Year, s.Month, day)))
                    .Count(date => date >= viewModel.PeriodStart && date <= viewModel.PeriodEnd));

                foreach (var employee in employees)
                {
                    if (workedDaysDict[employee.Id] > 0)
                    {
                        viewModel.Employees.Add(new EmployeeTipViewModel
                        {
                            EmployeeId = employee.Id,
                            EmployeeName = employee.Name,
                            DaysWorked = workedDaysDict[employee.Id]
                        });
                    }
                }

                if (!viewModel.Employees.Any())
                {
                    return (viewModel, "No employees found with worked days in the selected period.");
                }

                var totalDays = viewModel.Employees.Sum(e => e.DaysWorked);
                var tipPerDay = viewModel.TotalTipAmount / totalDays;

                foreach (var employee in viewModel.Employees)
                {
                    employee.TipAmount = Math.Round(tipPerDay * employee.DaysWorked, 2);
                }

                var totalDistributed = viewModel.Employees.Sum(e => e.TipAmount);
                if (totalDistributed != viewModel.TotalTipAmount)
                {
                    var difference = viewModel.TotalTipAmount - totalDistributed;
                    viewModel.Employees[0].TipAmount += difference;
                }

                return (viewModel, string.Empty);
            }
            catch (BusinessNotFoundException)
            {
                return (viewModel, "An error occurred while calculating tips due to a missing business.");
            }
            catch (Exception)
            {
                return (viewModel, "An error occurred while calculating tips.");
            }
        }
    }
} 