using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using DotnetAPI.Models;
using DotnetAPI.DTOs;

namespace DotnetAPI.Services
{
  public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto model)
    {
        if (model.Password != model.ConfirmPassword)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Passwords do not match." });
        }

        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };

        return await _userManager.CreateAsync(user, model.Password);
    }
}
}