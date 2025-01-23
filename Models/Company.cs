namespace DotnetAPI.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<CompanyEmployee> CompanyEmployees { get; set; } = new List<CompanyEmployee>(); // Link to CompanyEmployee
    }
}
