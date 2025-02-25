using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CompanyManagement.DTOs;

namespace CompanyManagement.Services
{
    public interface IUploadImageService
    {
        Task<ImageDto> UploadImageAsync(int employeeId, IFormFile imageFile, CancellationToken cancellationToken);
        Task<List<ImageDto>> GetEmployeeImagesAsync(int employeeId);
        Task<bool> DeleteImageAsync(int imageId, CancellationToken cancellationToken);
    }
}
