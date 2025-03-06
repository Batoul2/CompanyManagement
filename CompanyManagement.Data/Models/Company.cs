using System.Collections.Generic;
using CompanyManagement.Data.JunctionModels;

namespace CompanyManagement.Data.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<CompanyEmployee> CompanyEmployee { get; set; } = new List<CompanyEmployee>(); // Link to CompanyEmployee
    }
}
