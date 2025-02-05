using CompanyManagement.DTOs;
using CompanyManagement.InputModels;
using CompanyManagement.Models;
using CompanyManagement.QueryParameters;
using CompanyManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace CompanyManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredProjects([FromQuery] ProjectQueryParameters parameters,CancellationToken cancellationToken)
        {
            var projects = await _projectService.GetFilteredProjectsAsync(parameters,cancellationToken);

            if (projects == null || !projects.Any())
            {
                  return Ok(new Project[] { });
            }

            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProjectById([FromRoute]int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);

            if (project == null)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Project with ID {id} not found."
                });
            }

            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(ProjectInputModel inputModel,CancellationToken cancellationToken)
        {
            var createdProject = await _projectService.AddProjectAsync(inputModel, cancellationToken);
            return CreatedAtAction(nameof(GetProjectById), new { id = createdProject.Id }, createdProject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, ProjectInputModel inputModel,CancellationToken cancellationToken)
        {
            var updatedProject = await _projectService.UpdateProjectAsync(id, inputModel, cancellationToken);

            if (updatedProject == null)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Project with ID {id} not found."
                });
            }

            return Ok(updatedProject); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id,CancellationToken cancellationToken)
        {
            var success = await _projectService.DeleteProjectAsync(id, cancellationToken);

            if (!success)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Project with ID {id} not found."
                });
            }

            return NoContent();
        }

    }
}
