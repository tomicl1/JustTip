using System.ComponentModel.DataAnnotations;

namespace JustTip.Core.Models
{
    public class Business
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public List<Employee> Employees { get; set; } = new();

        public int GetEmployeeCount()
        {
            return Employees.Count;
        }
    }
} 