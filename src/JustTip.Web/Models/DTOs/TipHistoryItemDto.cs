namespace JustTip.Web.Models.DTOs
{
    public class TipHistoryItemDto
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string DistributionType { get; set; } = string.Empty;
    }
} 