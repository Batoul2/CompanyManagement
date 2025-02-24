using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CompanyManagement.Services
{
    public class FileService : IFileService
    {
        private readonly string _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("Invalid file.");

                string uploadPath = Path.Combine(_basePath, folderName);
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                string filePath = Path.Combine(uploadPath, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return Path.Combine("uploads", folderName, uniqueFileName).Replace("\\", "/");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FileService Error: {ex.Message}");
                throw new Exception("Error saving file. Check logs for details.");
            }
        }
    }
}
