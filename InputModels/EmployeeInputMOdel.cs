namespace CompanyManagement.InputModels
{
  public class EmployeeInputModel
  {
      public string FullName { get; set; } = string.Empty;
      public string Position { get; set; } = string.Empty;
      public List<int> CompanyIds { get; set; } = new List<int>();  
      public List<int> ProjectIds { get; set; } = new List<int>();  
      public IFormFile? ProfilePicture { get; set; }
  }


}