namespace CompanyManagement.API.InputModels
{
  public class ProjectInputModel
  {
      public string Title { get; set; } = string.Empty;
      public TimeSpan Duration { get; set; }
  }

}