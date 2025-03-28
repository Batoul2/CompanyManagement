using System.Collections.Generic;
using CompanyManagement.Data.JunctionModels;

namespace CompanyManagement.Data.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public ICollection<CompanyEmployee> CompanyEmployee { get; set; } = new List<CompanyEmployee>(); // Link to CompanyEmployee
        public ICollection<EmployeeProject> EmployeeProject { get; set; } = new List<EmployeeProject>(); // Link to EmployeeProject
        public List<Image> Images { get; set; } = new List<Image>();
    }
}
