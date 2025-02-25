using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using CompanyManagement.Services;

namespace CompanyManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadImageController : ControllerBase
    {
        private readonly IUploadImageService _uploadImageService;

        public UploadImageController(IUploadImageService uploadImageService)
        {
            _uploadImageService = uploadImageService;
        }

        [HttpPost("{id}/upload-profile-pictures")]
        public async Task<IActionResult> UploadProfilePictures([FromRoute] int id, [FromForm] List<IFormFile> profilePictures, CancellationToken cancellationToken)
        {
            if (profilePictures == null || profilePictures.Count == 0)
            {
                return BadRequest("At least one profile picture is required.");
            }

            var imagePaths = await _uploadImageService.UploadProfilePicturesAsync(id, profilePictures, cancellationToken);

            return Ok(new { Message = "Profile pictures uploaded successfully", ImagePaths = imagePaths });
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeImages([FromRoute] int employeeId)
        {
            var images = await _uploadImageService.GetEmployeeImagesAsync(employeeId);
            return Ok(new { EmployeeId = employeeId, Images = images });
        }

        [HttpDelete("{employeeId}/delete")]
        public async Task<IActionResult> DeleteEmployeeImages([FromRoute] int employeeId, CancellationToken cancellationToken)
        {
            var success = await _uploadImageService.DeleteEmployeeImagesAsync(employeeId, cancellationToken);
            if (!success)
                return NotFound(new { Message = "No images found for this employee." });

            return Ok(new { Message = "All images deleted successfully." });
        }
    }
}
