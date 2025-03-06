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
        Task<bool> DeleteImageAsync(int imageId, CancellationToken cancellationToken);
        Task<(string fileName, byte[] fileContent)> GetImageAsync(int imageId, CancellationToken cancellationToken);
    }
}
