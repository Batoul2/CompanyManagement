using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DotnetAPI.Models;
using DotnetAPI.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace DotnetAPI.Services
{
  public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;
    

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration,RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _configuration = configuration;
        _roleManager = roleManager;
    }
    //performed the cancellation token here
    public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto model, CancellationToken cancellationToken)
    {
        if (model.Password != model.ConfirmPassword)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Passwords do not match." });
        }

        cancellationToken.ThrowIfCancellationRequested();

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Email is already taken." });
        }
        cancellationToken.ThrowIfCancellationRequested();

        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };

        return await _userManager.CreateAsync(user, model.Password);
    }

    public async Task<string?> LoginAsync(LoginUserDto model, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByNameAsync(model.Username);
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return null;
        }

        return await GenerateJwtTokenAsync(user, cancellationToken);
    }

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user, CancellationToken cancellationToken)
        {

            cancellationToken.ThrowIfCancellationRequested(); 
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await _userManager.GetRolesAsync(user);
            cancellationToken.ThrowIfCancellationRequested(); 
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Generate the signing key
#pragma warning disable CS8604 // Possible null reference argument.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
#pragma warning restore CS8604 // Possible null reference argument.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> AssignRoleToUserAsync(string username, string role, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(username);
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                return "User not found.";
            }

            var roleExists = await _roleManager.RoleExistsAsync(role);
            cancellationToken.ThrowIfCancellationRequested();
            if (!roleExists)
            {
                return "Role not found.";
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            cancellationToken.ThrowIfCancellationRequested();
            if (result.Succeeded)
            {
                return $"Role {role} assigned to {username}.";
            }

            return "Error assigning role.";
        }

        public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                return null;
            }

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto model, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid email." });
            }

            return await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        }

    }
}