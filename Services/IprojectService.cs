using DotnetAPI.DTOs;
using DotnetAPI.InputModels;

namespace DotnetAPI.Services
{
    public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetFilteredProjectsAsync(string? title);
    Task<ProjectDto> GetProjectByIdAsync(int id);
    Task<ProjectDto> AddProjectAsync(ProjectInputModel inputModel);
    Task<ProjectDto> UpdateProjectAsync(int id, ProjectInputModel inputModel);
    Task<bool> DeleteProjectAsync(int id);
} 


}