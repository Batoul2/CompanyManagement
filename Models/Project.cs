namespace DotnetAPI.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public ICollection<EmployeeProject> EmployeeProject { get; set; } = new List<EmployeeProject>(); // Link to EmployeeProject
    }
}
