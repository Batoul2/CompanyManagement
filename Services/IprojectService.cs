using CompanyManagement.DTOs;
using CompanyManagement.InputModels;
using CompanyManagement.QueryParameters;

namespace CompanyManagement.Services
{
    public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetFilteredProjectsAsync(ProjectQueryParameters parameters,CancellationToken cancellationToken);
    Task<ProjectDto?> GetProjectByIdAsync(int id);
    Task<ProjectDto> AddProjectAsync(ProjectInputModel inputModel, CancellationToken cancellationToken);
    Task<ProjectDto?> UpdateProjectAsync(int id, ProjectInputModel inputModel,CancellationToken cancellationToken);
    Task<bool> DeleteProjectAsync(int id,CancellationToken cancellationToken);
    Task<bool> ProjectTitleExistsAsync(string title, int excludeId, CancellationToken cancellationToken);
}


}