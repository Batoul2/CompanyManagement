using Microsoft.AspNetCore.Mvc;
using CompanyManagement.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using CompanyManagement.API.Services;

namespace CompanyManagement.API.Controllers
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
        public async Task<IActionResult> Login([FromBody] LoginUserDto model,  CancellationToken cancellationToken)
        {
            var token = await _authService.LoginAsync(model, cancellationToken);
            if (token == null)
            {
                return Unauthorized("Invalid credentials.");
            }
            

            return Ok(new { Token = token });
        }

        // [Authorize(Roles = "Admin")]
        // [HttpPost("assign-role")]
        [HttpPost("assign-role"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleDto model)
        {
            // var userClaims = HttpContext.User.Claims.ToList();
            // Console.WriteLine("User Claims:");
            // foreach (var claim in userClaims)
            // {
            //     Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            // }

            // if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Role))
            // {
            //     return BadRequest("Username and Role are required.");
            // }
            // var result = await _authService.AssignRoleToUserAsync(model);

            // return Ok(result); 
            var result = await _authService.AssignRoleToUserAsync(model);
            if (result.Contains("User not found") || result.Contains("Role does not exist"))
                return BadRequest(new { message = result });

            return Ok(new { message = result });
        }

        [HttpGet("admin-only"), Authorize(Roles = "Admin")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok(new { message = "Only admins can access this!" });
        }
        

        [HttpPost("RequestPasswordReset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto model, CancellationToken cancellationToken)
        {
            var result = await _authService.RequestPasswordResetAsync(model.Email, cancellationToken);
            if (!result)
            {
                return BadRequest("If the email exists, a reset link will be sent.");
            }

            return Ok("Password reset email sent.");
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model, CancellationToken cancellationToken)
        {
            var result = await _authService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword, cancellationToken);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok("Password reset successful.");
        }

        [HttpGet("ResetPasswordPage")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ResetPasswordPage([FromQuery] string email, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid password reset request.");
            }
            var htmlResponse = $@"
                <html>
                <head><title>Reset Password Token</title></head>
                <body>
                    <h2>Copy Your Password Reset Token</h2>
                    <p><strong>Email:</strong> {email}</p>
                    <p><strong>Token:</strong> <code>{token}</code></p>
                    <p>Use this token in Swagger to reset your password.</p>
                </body>
                </html>";

            return Content(htmlResponse, "text/html");
        }


  }
  
}