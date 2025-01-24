using Microsoft.AspNetCore.Identity;

namespace DotnetAPI.Models
{
  public class ApplicationUser : IdentityUser
  {
      // IdentityUser already includes properties like Id, UserName, Email
  }
}