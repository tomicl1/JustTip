using System.ComponentModel.DataAnnotations;

namespace JustTip.Core.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public int BusinessId { get; set; }

        [Required]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;

        [StringLength(100)]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public DateTime HireDate { get; set; }

        public Business Business { get; set; } = null!;

        public List<TipAllocation> TipAllocations { get; set; } = new List<TipAllocation>();
        public List<Shift> Shifts { get; set; } = new List<Shift>();
    }
} 