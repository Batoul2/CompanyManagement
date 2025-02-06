using System.Threading.Tasks;

namespace CompanyManagement.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
