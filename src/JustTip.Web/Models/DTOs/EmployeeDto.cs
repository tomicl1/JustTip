using System.ComponentModel.DataAnnotations;

namespace JustTip.Web.Models.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public decimal? LastTipAmount { get; set; }
        public ICollection<TipAllocationDto> TipAllocations { get; set; } = new List<TipAllocationDto>();
    }
} 