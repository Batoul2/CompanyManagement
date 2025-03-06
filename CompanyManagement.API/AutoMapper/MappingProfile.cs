using AutoMapper;
using CompanyManagement.Data.Models;
using CompanyManagement.API.DTOs;
using CompanyManagement.API.InputModels;

namespace CompanyManagement.API.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Project, ProjectDto>().ReverseMap();

            CreateMap<CompanyInputModel, Company>();
            CreateMap<EmployeeInputModel, Employee>();
            CreateMap<ProjectInputModel, Project>();
        }
    }
}
