using DotnetAPI.DTOs;
using DotnetAPI.InputModels;

namespace DotnetAPI.Services
{
    public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetFilteredProjectsAsync(string? title,CancellationToken cancellationToken);
    Task<ProjectDto> GetProjectByIdAsync(int id);
    Task<ProjectDto> AddProjectAsync(ProjectInputModel inputModel, CancellationToken cancellationToken);
    Task<ProjectDto> UpdateProjectAsync(int id, ProjectInputModel inputModel,CancellationToken cancellationToken);
    Task<bool> DeleteProjectAsync(int id,CancellationToken cancellationToken);
} 


}