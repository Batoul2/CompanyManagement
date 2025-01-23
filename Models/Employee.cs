namespace DotnetAPI.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public ICollection<CompanyEmployee> CompanyEmployees { get; set; } = new List<CompanyEmployee>(); // Link to CompanyEmployee
        public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>(); // Link to EmployeeProject
    }
}
