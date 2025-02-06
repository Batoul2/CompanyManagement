using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using CompanyManagement.Models;
using CompanyManagement.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using MailKit;

namespace CompanyManagement.Services
{
  public class AuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    
    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration,RoleManager<IdentityRole> roleManager, IEmailService emailService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _emailService = emailService;
    }
    public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto model,  CancellationToken cancellationToken)
    {
        if (model.Password != model.ConfirmPassword)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Passwords do not match." });
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Email is already taken." });
        }

        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };

        //return await _userManager.CreateAsync(user, model.Password);
        var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

        return result;
    }

    public async Task<string> LoginAsync(LoginUserDto model,  CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return "Invalid Credentials";
        }
        // var userRoles = await _userManager.GetRolesAsync(user);
        // if (userRoles.Contains("Admin"))
        // {
        //     return await GenerateJwtTokenAsync(user);
        // }

        // return null;
        return GenerateJwtToken(user);
    }

    public async Task<string> AssignRoleToUserAsync(AssignRoleDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return "User not found.";
            }

            var roleExists = await _roleManager.RoleExistsAsync(model.Role);
            if (!roleExists)
            {
                return "Role not found.";
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (result.Succeeded)
            {
                return $"Role {model.Role} assigned to {model.Username}.";
            }

            return "Error assigning role.";
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var userRoles = _userManager.GetRolesAsync(user).Result;
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

#pragma warning disable CS8604 // Possible null reference argument.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
#pragma warning restore CS8604 // Possible null reference argument.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    // private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    // {
    //     var claims = new List<Claim>
    //     {
    //         new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
    //         new Claim(ClaimTypes.NameIdentifier, user.Id) 
    //     };

    //     var roles = await _userManager.GetRolesAsync(user);
    //     //claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    //     foreach (var role in roles)
    //     {
    //         claims.Add(new Claim(ClaimTypes.Role, role)); 
    //     }
        
    //     var secretKey = _configuration["Jwt:SecretKey"];
    //     if (string.IsNullOrEmpty(secretKey))
    //     {
    //         throw new InvalidOperationException("JWT SecretKey is not configured.");
    //     }

    //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //     var token = new JwtSecurityToken(
    //         issuer: _configuration["Jwt:Issuer"],
    //         audience: _configuration["Jwt:Audience"],
    //         claims: claims,
    //         expires: DateTime.Now.AddMinutes(60),
    //         signingCredentials: creds
    //     );

    //     // var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    //     // Console.WriteLine($"Generated Token: {tokenString}");  

    //     // return tokenString;
    //     return new JwtSecurityTokenHandler().WriteToken(token);
    // }

        // public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        // {
        //     var userRoles = await _userManager.GetRolesAsync(user);
        //     var authClaims = new List<Claim>
        //     {
        //         new Claim(ClaimTypes.Name, user.UserName?? string.Empty),
        //         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //     };

        //     foreach (var userRole in userRoles)
        //     {
        //         authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        //     }

        //     var jwtKey = _configuration["Jwt:SecretKey"];

        //     if (string.IsNullOrEmpty(jwtKey))
        //     {
        //         throw new InvalidOperationException("JWT Secret Key is missing in configuration.");
        //     }

        //     var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        //     var token = new JwtSecurityToken(
        //         issuer: _configuration["Jwt:Issuer"],
        //         audience: _configuration["Jwt:Audience"],
        //         expires: DateTime.Now.AddDays(3),
        //         claims: authClaims,
        //         signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //     );

        //     return new JwtSecurityTokenHandler().WriteToken(token);
        // }



        public async Task<bool> RequestPasswordResetAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"http://localhost:5292/api/auth/ResetPasswordPage?email={email}&token={Uri.EscapeDataString(token)}";

            var subject = "Password Reset Request";
            var body = $"Click <a href='{resetLink}'>here</a> to reset your password.";

            await _emailService.SendEmailAsync(email, subject, body);

            return true;
        }


        public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "Invalid email" });

            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

    }
}