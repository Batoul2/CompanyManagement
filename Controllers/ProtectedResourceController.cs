using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProtectedResourceController : ControllerBase
    {
        // This endpoint is protected and requires a valid JWT token to access
        [Authorize]
        [HttpGet("protected-resource")]
        public IActionResult GetProtectedResource()
        {
            return Ok("This is a protected resource.");
        }
    }
}
