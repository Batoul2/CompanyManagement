using CompanyManagement.Data.Models;

namespace CompanyManagement.Data.JunctionModels
{
    public class CompanyEmployee
    {
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
    }
}
