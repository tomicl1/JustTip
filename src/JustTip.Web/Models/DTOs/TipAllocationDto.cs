namespace JustTip.Web.Models.DTOs
{
    public class TipAllocationDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DistributionDate { get; set; }
    }
} 