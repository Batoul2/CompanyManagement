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

    public async Task<string?> LoginAsync(LoginUserDto model)
    {
        // Find user by username (or email)
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return null; 
        }

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(ApplicationUser user)
        {
            // Create claims for the token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            // Add user roles to the claims
            var roles = _userManager.GetRolesAsync(user).Result;
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Generate the signing key
#pragma warning disable CS8604 // Possible null reference argument.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
#pragma warning restore CS8604 // Possible null reference argument.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> AssignRoleToUserAsync(string username, string role)
        {
            // Check if the user exists
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return "User not found.";
            }

            // Check if the role exists
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return "Role not found.";
            }

            // Assign the role to the user
            var result = await _userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                return $"Role {role} assigned to {username}.";
            }

            return "Error assigning role.";
        }
}
}