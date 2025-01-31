using AutoMapper;
using JustTip.Core.Interfaces;
using JustTip.Core.Models;
using JustTip.Web.Models.DTOs;
using JustTip.Web.Models.ViewModels;
using JustTip.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JustTip.Core.Exceptions;

namespace JustTip.Web.Controllers
{
    public class EmployeeController : Controller, IEmployeeController
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IShiftRepository _shiftRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(
            IEmployeeRepository employeeRepository,
            IBusinessRepository businessRepository,
            IShiftRepository shiftRepository,
            IMapper mapper,
            ILogger<EmployeeController> logger,
            IEmployeeService employeeService)
        {
            _employeeRepository = employeeRepository;
            _businessRepository = businessRepository;
            _shiftRepository = shiftRepository;
            _mapper = mapper;
            _logger = logger;
            _employeeService = employeeService;
        }

        // GET: Employee/Business/5
        public IActionResult Index(int businessId)
        {
            try
            {
                var business = _businessRepository.GetById(businessId);
                if (business == null)
                {
                    throw new BusinessNotFoundException(businessId);
                }

                var employees = _employeeRepository.GetEmployees(businessId)
                    .Include(e => e.TipAllocations)
                    .ToList();

                var employeeDtos = _mapper.Map<List<EmployeeDto>>(employees);
                foreach (var dto in employeeDtos)
                {
                    var lastTip = employees
                        .First(e => e.Id == dto.Id)
                        .TipAllocations
                        .OrderByDescending(t => t.DistributionDate)
                        .FirstOrDefault();
                    dto.LastTipAmount = lastTip?.Amount;
                }

                var viewModel = new EmployeeListViewModel
                {
                    BusinessId = businessId,
                    BusinessName = business.Name,
                    Employees = employeeDtos
                };

                return View(viewModel);
            }
            catch (BusinessNotFoundException ex)
            {
                _logger.LogError(ex, "Business not found.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving employees.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // GET: Employee/Details/5
        public IActionResult Details(int businessId, int id)
        {
            var employee = _employeeRepository.GetEmployee(businessId, id);
            if (employee == null)
            {
                return NotFound();
            }

            var business = _businessRepository.GetById(businessId);
            
            var viewModel = new EmployeeDetailsViewModel
            {
                Employee = _mapper.Map<EmployeeDto>(employee),
                BusinessId = businessId,
                BusinessName = business?.Name ?? "Unknown Business",
                TipHistory = employee.TipAllocations
                    .OrderByDescending(t => t.DistributionDate)
                    .Select(t => new TipHistoryItemDto
                    {
                        Date = t.DistributionDate,
                        Amount = t.Amount
                    })
                    .ToList()
            };

            return View(viewModel);
        }

        // GET: Employee/Create/5
        [HttpGet]
        public IActionResult Create(int businessId)
        {
            var business = _businessRepository.GetById(businessId);
            if (business == null)
            {
                return NotFound();
            }

            var employeeDto = new EmployeeDto { BusinessId = businessId };
            return View(employeeDto);
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeDto employeeDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var employee = _mapper.Map<Employee>(employeeDto);
                    await _employeeRepository.AddEmployeeAsync(employeeDto.BusinessId, employee);
                    return RedirectToAction(nameof(Index), new { businessId = employeeDto.BusinessId });
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Validation error creating employee");
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating employee");
                    ModelState.AddModelError("", "Unable to create employee. Please try again.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(employeeDto);
        }

        // GET: Employee/Delete/5
        public IActionResult Delete(int businessId, int id)
        {
            var employee = _employeeRepository.GetEmployee(businessId, id);
            if (employee == null)
            {
                return NotFound();
            }

            var business = _businessRepository.GetById(businessId);
            var viewModel = new EmployeeDetailsViewModel
            {
                Employee = _mapper.Map<EmployeeDto>(employee),
                BusinessId = businessId,
                BusinessName = business?.Name ?? "Unknown Business",
                TipHistory = employee.TipAllocations
                    .OrderByDescending(t => t.DistributionDate)
                    .Select(t => new TipHistoryItemDto
                    {
                        Date = t.DistributionDate,
                        Amount = t.Amount
                    })
                    .ToList()
            };

            return View(viewModel);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int businessId, int id)
        {
            try
            {
                var employee = _employeeRepository.GetEmployee(businessId, id);
                if (employee != null)
                {
                    await _employeeRepository.DeleteEmployeeAsync(businessId, id);
                }
                return RedirectToAction(nameof(Index), new { businessId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee");
                return RedirectToAction(nameof(Index), new { businessId });
            }
        }

        // GET: Employee/Edit/5
        [HttpGet]
        public IActionResult Edit(int businessId, int id)
        {
            var employee = _employeeRepository.GetEmployee(businessId, id);
            if (employee == null)
            {
                return NotFound();
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return View(employeeDto);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeDto employeeDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var employee = _employeeRepository.GetEmployee(employeeDto.BusinessId, employeeDto.Id);
                    if (employee == null)
                    {
                        return NotFound();
                    }

                    _mapper.Map(employeeDto, employee);
                    await _employeeRepository.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { businessId = employeeDto.BusinessId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating employee");
                    ModelState.AddModelError("", "Unable to update employee. Please try again.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(employeeDto);
        }

        // GET: Employee/ManageShifts/5
        public IActionResult ManageShifts(int businessId, int id, int? year = null, int? month = null)
        {
            var employee = _employeeRepository.GetEmployee(businessId, id);
            if (employee == null)
            {
                return NotFound();
            }

            year ??= DateTime.Today.Year;
            month ??= DateTime.Today.Month;

            var viewModel = new ManageShiftsViewModel
            {
                EmployeeId = id,
                BusinessId = businessId,
                EmployeeName = employee.Name,
                Year = year.Value,
                Month = month.Value
            };

            // Get existing shift record for this month
            var shift = employee.Shifts
                .FirstOrDefault(s => s.Year == year && s.Month == month);

            var workedDays = shift?.GetWorkedDays() ?? new List<int>();
            var daysInMonth = DateTime.DaysInMonth(year.Value, month.Value);

            // Create entries for each day in the month
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year.Value, month.Value, day);
                viewModel.Days.Add(new DayViewModel
                {
                    Day = day,
                    IsWorked = workedDays.Contains(day),
                    DayName = date.ToString("ddd")
                });
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageShifts(int businessId, int id, [Bind("EmployeeId,BusinessId,Year,Month,Days")] ManageShiftsViewModel viewModel)
        {
            if (id != viewModel.EmployeeId || businessId != viewModel.BusinessId)
            {
                return NotFound();
            }

            var employee = _employeeRepository.GetEmployee(businessId, id);
            if (employee == null)
            {
                return NotFound();
            }

            // Find or create shift record
            var shift = await _shiftRepository.GetShifts()
                .FirstOrDefaultAsync(s => 
                    s.EmployeeId == id && 
                    s.Year == viewModel.Year && 
                    s.Month == viewModel.Month);

            var workedDays = viewModel.Days
                .Where(d => d.IsWorked)
                .Select(d => d.Day)
                .OrderBy(d => d)
                .ToList();

            if (workedDays.Count != 0)
            {
                if (shift == null)
                {
                    shift = new Shift
                    {
                        EmployeeId = id,
                        Year = viewModel.Year,
                        Month = viewModel.Month
                    };
                    _shiftRepository.AddShift(shift);
                }
                
                shift.SetWorkedDays(workedDays);
            }
            else if (shift != null)
            {
                _shiftRepository.RemoveShift(shift);
            }

            try
            {
                await _employeeRepository.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { businessId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving shift data");
                ModelState.AddModelError("", "Unable to save shift data. Please try again.");
                
                // Reload the view with current data
                viewModel.EmployeeName = employee.Name;
                return View(viewModel);
            }
        }

        public async Task<IActionResult> Edit(int businessId, EmployeeDto employeeDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.UpdateEmployeeAsync(businessId, employeeDto);
                    return RedirectToAction(nameof(Index), new { businessId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating employee");
                    ModelState.AddModelError("", "Unable to update employee. Please try again.");
                }
            }

            var business = _businessRepository.GetById(businessId);
            return View(employeeDto);
        }
    }
} 