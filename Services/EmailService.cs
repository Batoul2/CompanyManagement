using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using MailKit.Security;

namespace CompanyManagement.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new InvalidOperationException("SMTP password is missing. Set the SMTP_PASSWORD environment variable.");
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var enableSSL = bool.Parse(_configuration["EmailSettings:EnableSSL"] ?? "true");

            if (string.IsNullOrEmpty(smtpPass))
            {
                throw new InvalidOperationException("SMTP password is missing. Set the SMTP_PASSWORD environment variable.");
            }

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Company Support", fromEmail));
            email.To.Add(new MailboxAddress(to, to));
            email.Subject = subject;

            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpUser, smtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
