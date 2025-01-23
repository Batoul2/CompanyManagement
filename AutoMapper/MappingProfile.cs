using AutoMapper;
using DotnetAPI.Models;
using DotnetAPI.DTOs;
using DotnetAPI.InputModels;

namespace DotnetAPI.AutoMapperProfiles
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
