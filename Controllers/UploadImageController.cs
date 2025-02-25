using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using CompanyManagement.Services;
using CompanyManagement.InputModels;
using CompanyManagement.DTOs;

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

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromRoute] int employeeId, [FromForm] ImageInputModel inputModel, CancellationToken cancellationToken)
        {
            var image = await _uploadImageService.UploadImageAsync(employeeId, inputModel.ImageFile, cancellationToken);
            return Ok(image);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeImages([FromRoute] int employeeId)
        {
            var images = await _uploadImageService.GetEmployeeImagesAsync(employeeId);
            return Ok(images);
        }

        [HttpDelete("delete/{imageId}")]
        public async Task<IActionResult> DeleteImage([FromRoute] int imageId, CancellationToken cancellationToken)
        {
            await _uploadImageService.DeleteImageAsync(imageId, cancellationToken);
            return Ok(new { Message = $"Image with ID {imageId} deleted successfully." });
        }


    }
}
