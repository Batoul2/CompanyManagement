using Microsoft.AspNetCore.Http;

namespace CompanyManagement.API.InputModels
{
    public class ImageInputModel
    {
        public IFormFile ImageFile { get; set; } = null!;
        
    }
}
