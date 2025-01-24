using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Services;
using DotnetAPI.DTOs;

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
      public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
      {
          if (!ModelState.IsValid)
          {
              return BadRequest(ModelState);
          }

          var result = await _authService.RegisterUserAsync(model);

          if (!result.Succeeded)
          {
              return BadRequest(result.Errors);
          }

          return Ok(new { message = "User registered successfully!" });
      }
  }
}