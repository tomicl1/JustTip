using JustTip.Web.Models.DTOs;
using JustTip.Web.Models.ViewModels;
namespace JustTip.Web.Services
{
    public interface IBusinessService
    {
        Task<BusinessDto?> GetBusinessDtoByIdAsync(int id);
        Task<(TipDistributionViewModel ViewModel, string ErrorMessage)> CalculateTipsAsync(TipDistributionViewModel viewModel);
    }
} 