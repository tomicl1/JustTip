using JustTip.Web.Models.DTOs;

namespace JustTip.Web.Models.ViewModels
{
    public class EmployeeListViewModel
    {
        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public IEnumerable<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
    }
} 