namespace JustTip.Core.Models
{
    public class Shift
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string WorkedDays { get; set; } = string.Empty; // Stored as comma-separated days (1,2,3,15,22,.....)
        
        public Employee Employee { get; set; } = null!;

        public List<int> GetWorkedDays()
        {
            if (string.IsNullOrWhiteSpace(WorkedDays))
                return new List<int>();

            return WorkedDays.Split(',')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(int.Parse)
                .ToList();
        }

        public void SetWorkedDays(List<int> days)
        {
            WorkedDays = string.Join(",", days.OrderBy(d => d));
        }
    }
} 