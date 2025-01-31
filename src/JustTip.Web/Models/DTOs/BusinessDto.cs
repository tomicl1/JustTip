using System.ComponentModel.DataAnnotations;

namespace JustTip.Web.Models.DTOs
{
    public class BusinessDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Business Name")]
        public string Name { get; set; } = string.Empty;

        public int EmployeeCount { get; set; }

        public ICollection<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
    }
} 