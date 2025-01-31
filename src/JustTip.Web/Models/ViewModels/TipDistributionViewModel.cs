using System.ComponentModel.DataAnnotations;

namespace JustTip.Web.Models.ViewModels
{
    public class TipDistributionViewModel
    {
        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Period Start")]
        [DataType(DataType.Date)]
        public DateTime PeriodStart { get; set; } = DateTime.Today.AddDays(-7);
        
        [Required]
        [Display(Name = "Period End")]
        [DataType(DataType.Date)]
        public DateTime PeriodEnd { get; set; } = DateTime.Today;
        
        [Required]
        [Display(Name = "Total Tips")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a valid amount")]
        public decimal TotalTipAmount { get; set; }
        
        public List<EmployeeTipViewModel> Employees { get; set; } = new();
    }

    public class EmployeeTipViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int DaysWorked { get; set; }
        public decimal TipAmount { get; set; }
    }
} 