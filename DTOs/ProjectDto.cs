namespace CompanyManagement.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public IEnumerable<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
    }
}
