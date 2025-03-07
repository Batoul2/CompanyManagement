using CompanyManagement.API.InputModels;
using CompanyManagement.API.Validators;
using CompanyManagement.API.Services;
using CompanyManagement.API.AutoMapper;
using CompanyManagement.Data.Repositories;
using CompanyManagement.Utils;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyManagement.API.Configurations
{
    public static class ServiceConfiguration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddAutoMapper(typeof(MappingProfile));

            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<AuthService>();
            services.AddScoped<IUploadImageService, UploadImageService>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileService, FileService>();

            services.AddScoped<IValidator<EmployeeInputModel>, EmployeeValidator>();
            services.AddScoped<IValidator<ProjectInputModel>, ProjectValidator>();
            services.AddScoped<IValidator<CompanyInputModel>, CompanyValidator>();
        }
    }
}
