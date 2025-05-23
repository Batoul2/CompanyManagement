namespace CompanyManagement.API.DTOs
{
  public class RegisterUserDto
  {
      public string Username { get; set; } = string.Empty;
      public string Email { get; set; } = string.Empty;
      public string Password { get; set; } = string.Empty;
      public string ConfirmPassword { get; set; } = string.Empty;
      public string PhoneNumber { get; set; } = string.Empty;
  }

}