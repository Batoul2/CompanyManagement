namespace DotnetAPI.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public IEnumerable<CompanyDto> Companies { get; set; } = new List<CompanyDto>();
        public IEnumerable<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
    }

}
