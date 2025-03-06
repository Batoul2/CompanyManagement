using System.ComponentModel.DataAnnotations;

namespace CompanyManagement.API.QueryParameters
{
    public class EmployeeQueryParameters
    {
        public string? SearchTerm { get; set; } 
        [Required]
        public int Page { get; set; } = 1; 
        [Required]
        public int PageSize { get; set; } = 10; 
        [Required]
        public string SortBy { get; set; } = "FullName"; 
        [Required]
        public string SortDir { get; set; } = "asc"; 

        public int GetSkipAmount() => (Page - 1) * PageSize;
    }
}
