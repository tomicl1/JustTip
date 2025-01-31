using JustTip.Web.Models.DTOs;
using JustTip.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace JustTip.Web.Controllers
{
    public interface IBusinessController
    {
        Task<IActionResult> Index();
        Task<IActionResult> Details(int id);
        IActionResult Create();
        Task<IActionResult> Create(BusinessDto businessDto);
        IActionResult Edit(int id);
        Task<IActionResult> Edit(int id, BusinessDto businessDto);
        IActionResult Delete(int id);
        Task<IActionResult> DeleteConfirmed(int id);
        IActionResult TipDistribution(int id);
        Task<IActionResult> CalculateTips(TipDistributionViewModel viewModel);
        Task<IActionResult> SaveTipDistribution(TipDistributionViewModel viewModel);
    }
} 