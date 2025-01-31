using System.ComponentModel.DataAnnotations;

namespace JustTip.Core.Models
{
    public class TipAllocation
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DistributionDate { get; set; }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int DaysWorked { get; set; }

        public Employee Employee { get; set; } = null!;
    }
} 