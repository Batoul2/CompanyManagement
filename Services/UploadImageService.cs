using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CompanyManagement.Data;
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

        public async Task<List<string>> UploadProfilePicturesAsync(int employeeId, List<IFormFile> profilePictures, CancellationToken cancellationToken)
        {
            var employee = await _dbContext.Employees.FindAsync(new object[] { employeeId }, cancellationToken);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
            }
            if (employee.ProfilePicturePaths == null)
            {
                employee.ProfilePicturePaths = new List<string>();
            }

            List<string> uploadedImagePaths = new();

            foreach (var file in profilePictures)
            {
                var filePath = await _fileService.SaveFileAsync(file);
                uploadedImagePaths.Add(filePath);
            }

            // Append the new images to existing images
            employee.ProfilePicturePaths.AddRange(uploadedImagePaths);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return uploadedImagePaths;
        }

        public async Task<List<string>> GetEmployeeImagesAsync(int employeeId)
        {
            var employee = await _dbContext.Employees.FindAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");

            return employee.ProfilePicturePaths ?? new List<string>();
        }

        public async Task<bool> DeleteEmployeeImagesAsync(int employeeId, CancellationToken cancellationToken)
        {
            var employee = await _dbContext.Employees.FindAsync(new object[] { employeeId }, cancellationToken);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");

            if (employee.ProfilePicturePaths == null || employee.ProfilePicturePaths.Count == 0)
                return false;

            foreach (var imagePath in employee.ProfilePicturePaths)
            {
                await _fileService.DeleteFileAsync(imagePath);
            }

            employee.ProfilePicturePaths.Clear();
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

}
