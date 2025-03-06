using CompanyManagement.Data.Models;

namespace CompanyManagement.Data.JunctionModels
{
    public class EmployeeProject
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
    }
}
