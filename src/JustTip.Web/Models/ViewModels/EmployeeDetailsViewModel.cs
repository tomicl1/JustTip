using JustTip.Web.Models.DTOs;

namespace JustTip.Web.Models.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public EmployeeDto Employee { get; set; } = new();
        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public IEnumerable<TipHistoryItemDto> TipHistory { get; set; } = new List<TipHistoryItemDto>();
    }
} 