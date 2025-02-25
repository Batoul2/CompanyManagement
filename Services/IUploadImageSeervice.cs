namespace CompanyManagement.Services
{
    public interface IUploadImageService
    {
        Task<List<string>> UploadProfilePicturesAsync(int employeeId, List<IFormFile> profilePictures, CancellationToken cancellationToken);
        Task<List<string>> GetEmployeeImagesAsync(int employeeId);
        Task<bool> DeleteEmployeeImagesAsync(int employeeId, CancellationToken cancellationToken);
    }
}