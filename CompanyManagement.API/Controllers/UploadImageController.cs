using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using CompanyManagement.API.Services;
using CompanyManagement.API.InputModels;
using CompanyManagement.API.DTOs;
using CompanyManagement.Data.Models;

namespace CompanyManagement.API.Controllers
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

        [HttpPost("{employeeId}/upload")]
        public async Task<IActionResult> UploadImage([FromRoute] int employeeId, [FromForm] ImageInputModel inputModel, CancellationToken cancellationToken)
        {
            if (employeeId <= 0)
            {
                return BadRequest(new { Message = "Invalid employee ID." });
            }

            var image = await _uploadImageService.UploadImageAsync(employeeId, inputModel.ImageFile, cancellationToken);
            return Ok(image);
        }

        [HttpGet("{imageId}")]
        public async Task<IActionResult> GetImage([FromRoute] int imageId, CancellationToken cancellationToken)
        {
            var (fileName, fileContent) = await _uploadImageService.GetImageAsync(imageId, cancellationToken);
            return File(fileContent, "image/jpeg", fileName);
        }


        [HttpDelete("delete/{imageId}")]
        public async Task<IActionResult> DeleteImage([FromRoute] int imageId, CancellationToken cancellationToken)
        {
            await _uploadImageService.DeleteImageAsync(imageId, cancellationToken);
            return Ok(new { Message = $"Image with ID {imageId} deleted successfully." });
        }


    }
}
