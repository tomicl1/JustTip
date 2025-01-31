using AutoMapper;
using JustTip.Core.Interfaces;
using JustTip.Core.Models;
using JustTip.Web.Models.DTOs;
using JustTip.Web.Models.ViewModels;
using JustTip.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JustTip.Web.Controllers
{
    public class BusinessController : Controller, IBusinessController
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly ITipRepository _tipRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BusinessController> _logger;
        private readonly IBusinessService _businessService;

        public BusinessController(
            IBusinessRepository repository,
            ITipRepository tipRepository,
            IMapper mapper,
            ILogger<BusinessController> logger,
            IBusinessService businessService)
        {
            _businessRepository = repository;
            _tipRepository = tipRepository;
            _mapper = mapper;
            _logger = logger;
            _businessService = businessService;
        }

        // GET: Business
        public async Task<IActionResult> Index()
        {
            var businesses = await _businessRepository.GetAll()
                .Include(b => b.Employees)
                .ToListAsync();

            var businessDtos = _mapper.Map<List<BusinessDto>>(businesses);
            return View(businessDtos);
        }

        // GET: Business/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var businessDto = await _businessService.GetBusinessDtoByIdAsync(id);
            if (businessDto == null)
            {
                return NotFound();
            }

            return View(businessDto);
        }

        // GET: Business/Create
        public IActionResult Create()
        {
            return View(new BusinessDto());
        }

        // POST: Business/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BusinessDto businessDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var business = _mapper.Map<Business>(businessDto);
                    await _businessRepository.AddAsync(business);
                    _logger.LogInformation("Business created successfully with ID {BusinessId}", business.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating business with name {BusinessName}", businessDto.Name);
                    ModelState.AddModelError("", "Unable to create business. Please try again.");
                }
            }

            return View(businessDto);
        }

        // GET: Business/Edit/5
        public IActionResult Edit(int id)
        {
            var business = _businessRepository.GetById(id);
            if (business == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<BusinessDto>(business));
        }

        // POST: Business/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BusinessDto businessDto)
        {
            if (id != businessDto.Id)
            {
                _logger.LogWarning("Edit attempted with mismatched ID: {Id} vs {BusinessDtoId}", id, businessDto.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var business = _mapper.Map<Business>(businessDto);
                    await _businessRepository.UpdateAsync(business);
                    _logger.LogInformation("Business updated successfully with ID {BusinessId}", business.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating business with ID {BusinessId}", businessDto.Id);
                    ModelState.AddModelError("", "Unable to update business. Please try again.");
                }
            }

            return View(businessDto);
        }

        // GET: Business/Delete/5
        public IActionResult Delete(int id)
        {
            var businessDto = _businessService.GetBusinessDtoByIdAsync(id).Result;
            if (businessDto == null)
            {
                return NotFound();
            }

            return View(businessDto);
        }

        // POST: Business/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _businessRepository.DeleteAsync(id);
                _logger.LogInformation("Business deleted successfully with ID {BusinessId}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting business with ID {BusinessId}", id);
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Business/TipDistribution/5
        public IActionResult TipDistribution(int id)
        {
            var business = _businessRepository.GetById(id);
            if (business == null)
            {
                return NotFound();
            }

            var viewModel = new TipDistributionViewModel
            {
                BusinessId = id,
                BusinessName = business.Name
            };

            return View(viewModel);
        }

        // POST: Business/CalculateTips
        [HttpPost]
        public async Task<IActionResult> CalculateTips(TipDistributionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("TipDistribution", viewModel);
            }

            try
            {
                var result = await _businessService.CalculateTipsAsync(viewModel);
                viewModel = result.ViewModel;
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    ModelState.AddModelError("", result.ErrorMessage);
                    return View("TipDistribution", viewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating tips: {Message}", ex.Message);
                ModelState.AddModelError("", "An unexpected error occurred while calculating tips.");
                return View("TipDistribution", viewModel);
            }

            return View("TipDistribution", viewModel);
        }

        // POST: Business/SaveTipDistribution
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTipDistribution(TipDistributionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("TipDistribution", viewModel);
            }

            try
            {
                foreach (var employee in viewModel.Employees)
                {
                    var tipAllocation = new TipAllocation
                    {
                        EmployeeId = employee.EmployeeId,
                        Amount = employee.TipAmount,
                        DistributionDate = DateTime.Now,
                        PeriodStart = viewModel.PeriodStart,
                        PeriodEnd = viewModel.PeriodEnd,
                        DaysWorked = employee.DaysWorked
                    };

                    await _tipRepository.AddTipAllocationAsync(tipAllocation);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving tip distribution");
                ModelState.AddModelError("", "Unable to save tip distribution. Please try again.");
                return View("TipDistribution", viewModel);
            }
        }
    }
} 