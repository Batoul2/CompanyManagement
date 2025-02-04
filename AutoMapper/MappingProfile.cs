using AutoMapper;
using CompanyManagement.Models;
using CompanyManagement.DTOs;
using CompanyManagement.InputModels;

namespace CompanyManagement.AutoMapper
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
