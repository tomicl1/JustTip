using System.Diagnostics;
using AutoMapper;
using JustTip.Core.Interfaces;
using JustTip.Web.Models;
using JustTip.Web.Models.DTOs;
using JustTip.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace JustTip.Web.Controllers;

public class HomeController : Controller, IHomeController
{
    private readonly IBusinessRepository _businessRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<HomeController> _logger;
    private readonly IHomeService _homeService;
    private readonly IMapper _mapper;

    public HomeController(
        IBusinessRepository businessRepository,
        IEmployeeRepository employeeRepository,
        ILogger<HomeController> logger,
        IHomeService homeService,
        IMapper mapper)
    {
        _businessRepository = businessRepository;
        _employeeRepository = employeeRepository;
        _logger = logger;
        _homeService = homeService;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        try
        {
            var businesses = _homeService.GetDashboardDataAsync().Result ?? new List<BusinessDto>();

            // Calculate some stats so that the homepage is not empty
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var lastMonth = thisMonth.AddMonths(-1);

            ViewBag.Stats = new
            {
                BusinessCount = businesses.Count(),
                EmployeeCount = businesses.Sum(b => b.EmployeeCount),
                ThisMonthTips = businesses
                    .SelectMany(b => b.Employees)
                    .SelectMany(e => e.TipAllocations)
                    .Where(t => t.DistributionDate >= thisMonth)
                    .Sum(t => t.Amount),
                LastMonthTips = businesses
                    .SelectMany(b => b.Employees)
                    .SelectMany(e => e.TipAllocations)
                    .Where(t => t.DistributionDate >= lastMonth && t.DistributionDate < thisMonth)
                    .Sum(t => t.Amount)
            };

            // Get recent tip distributions
            ViewBag.RecentTipAllocations = _homeService.GetRecentTipAllocationsAsync().Result ?? new List<TipAllocationSummaryDto>();

            return View(businesses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard data");
            return View(new List<BusinessDto>());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
