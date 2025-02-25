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

        public async Task<List<ImageDto>> GetEmployeeImagesAsync(int employeeId)
        {
            var images = await _dbContext.Images
                .Where(img => img.EmployeeId == employeeId)
                .Select(img => new ImageDto { Id = img.Id, ImagePath = img.ImagePath })
                .ToListAsync();

            return images;
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
