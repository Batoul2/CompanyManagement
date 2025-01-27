using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Services;
using DotnetAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DotnetAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
      private readonly AuthService _authService;

      public AuthController(AuthService authService)
      {
          _authService = authService;
      }

      [HttpPost("register")]
      public async Task<IActionResult> Register([FromBody] RegisterUserDto model,  CancellationToken cancellationToken)
      {
          if (!ModelState.IsValid)
          {
              return BadRequest(ModelState);
          }

          var result = await _authService.RegisterUserAsync(model, cancellationToken);

          if (!result.Succeeded)
          {
              return BadRequest(result.Errors);
          }

          return Ok(new { message = "User registered successfully!" });
      }

      [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto model)
        {
            var token = await _authService.LoginAsync(model);
            if (token == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok(new { Token = token });
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Role))
            {
                return BadRequest("Username and Role are required.");
            }

            var result = await _authService.AssignRoleToUserAsync(model.Username, model.Role);

            if (result.Contains("Error"))
            {
                return StatusCode(500, result); 
            }

            if (result.Contains("not found"))
            {
                return NotFound(result); 
            }

            return Ok(result); 
        }

        [HttpPost("RequestPasswordReset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto model)
        {
            var token = await _authService.GeneratePasswordResetTokenAsync(model.Email);
            if (token == null)
            {
                return BadRequest("If the email exists, a reset link will be sent.");
            }

            return Ok(new { Token = token });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var result = await _authService.ResetPasswordAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok("Password has been reset successfully.");
        }

  }
  
}