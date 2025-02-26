using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CompanyManagement.Data;
using CompanyManagement.DTOs;
using CompanyManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace CompanyManagement.Services
{
    public class UploadImageService : IUploadImageService
    {
        private readonly CompanyDbContext _dbContext;
        private readonly IFileService _fileService;
        private readonly string _uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages");

        public UploadImageService(CompanyDbContext dbContext, IFileService fileService)
        {
            _dbContext = dbContext;
            _fileService = fileService;
        }
        public async Task<ImageDto> UploadImageAsync(int employeeId, IFormFile imageFile, CancellationToken cancellationToken)
        {
            var employee = await _dbContext.Employees.FindAsync(new object[] { employeeId }, cancellationToken);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");

            var filePath = await _fileService.SaveFileAsync(imageFile);
            var image = new Image { ImagePath = filePath, EmployeeId = employeeId };

            _dbContext.Images.Add(image);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ImageDto { Id = image.Id, ImagePath = filePath };
        }

        public async Task<(string fileName, byte[] fileContent)> GetImageAsync(int imageId, CancellationToken cancellationToken)
        {
            var image = await _dbContext.Images.FirstOrDefaultAsync(i => i.Id == imageId, cancellationToken);
            if (image == null)
                throw new KeyNotFoundException($"Image with ID {imageId} not found.");

            var filePath = Path.Combine(_uploadFolderPath, image.ImagePath);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found at {filePath}");

            var fileContent = await File.ReadAllBytesAsync(filePath);
            return (image.ImagePath, fileContent);
        }

        public async Task<bool> DeleteImageAsync(int imageId, CancellationToken cancellationToken)
        {
            var image = await _dbContext.Images
                .FirstOrDefaultAsync(img => img.Id == imageId, cancellationToken);

            if (image == null)
                throw new KeyNotFoundException($"Image with ID {imageId} not found.");

            await _fileService.DeleteFileAsync(image.ImagePath);

            _dbContext.Images.Remove(image);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

    }
}
