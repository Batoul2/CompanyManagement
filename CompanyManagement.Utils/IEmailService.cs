using System.Threading.Tasks;

namespace CompanyManagement.Utils
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
