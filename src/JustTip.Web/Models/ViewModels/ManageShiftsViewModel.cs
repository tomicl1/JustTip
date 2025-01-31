using System.ComponentModel.DataAnnotations;

public class ManageShiftsViewModel
{
    public int EmployeeId { get; set; }
    public int BusinessId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    
    [Required]
    public int Year { get; set; }
    
    [Required]
    [Range(1, 12)]
    public int Month { get; set; }
    
    public List<DayViewModel> Days { get; set; } = new();
}

public class DayViewModel
{
    public int Day { get; set; }
    public bool IsWorked { get; set; }
    public string DayName { get; set; } = string.Empty;
} 