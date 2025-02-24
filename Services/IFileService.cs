namespace CompanyManagement.Services
{
      public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);
    }
}