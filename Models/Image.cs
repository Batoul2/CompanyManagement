namespace CompanyManagement.Models
{
    public class Image
    {
        public int Id { get; set; }  
        public string ImagePath { get; set; } = string.Empty; 
        public int EmployeeId { get; set; } 
        public Employee Employee { get; set; } = null!; 
    }
}
