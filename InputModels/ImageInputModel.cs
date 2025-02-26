using Microsoft.AspNetCore.Http;

namespace CompanyManagement.InputModels
{
    public class ImageInputModel
    {
        public IFormFile ImageFile { get; set; } = null!;
        
    }
}
