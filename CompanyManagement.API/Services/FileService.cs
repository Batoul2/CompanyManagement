using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CompanyManagement.API.Services
{
    public class FileService : IFileService
    {
        private readonly string _uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages");
        private const long MaxFileSize = 3 * 1024 * 1024;

        public FileService()
        {
            if (!Directory.Exists(_uploadFolderPath))
            {
                Directory.CreateDirectory(_uploadFolderPath);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            if (file.Length > MaxFileSize)
                throw new ArgumentException("File size exceeds the maximum limit of 3MB.");

            if (Path.GetExtension(file.FileName).ToLower() != ".jpg")
                throw new ArgumentException("Only .jpg files are allowed.");

            var fileName = $"{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(_uploadFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
        public Task DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_uploadFolderPath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return Task.CompletedTask;
        }
    }

    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
        Task DeleteFileAsync(string fileName);
    }
}
