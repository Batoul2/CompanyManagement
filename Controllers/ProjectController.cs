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
            var errors = new List<string>();

            var titleExists = await _projectService.ProjectTitleExistsAsync(inputModel.Title, 0, cancellationToken);
            if (titleExists)
                errors.Add("A project with this title already exists.");

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            var createdProject = await _projectService.AddProjectAsync(inputModel, cancellationToken);
            return CreatedAtAction(nameof(GetProjectById), new { id = createdProject.Id }, createdProject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject([FromRoute] int id, [FromBody] ProjectInputModel inputModel, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            var existingProject = await _projectService.GetProjectByIdAsync(id);
            if (existingProject == null)
                errors.Add($"Project with ID {id} not found.");

            var titleExists = await _projectService.ProjectTitleExistsAsync(inputModel.Title, id, cancellationToken);
            if (titleExists)
                errors.Add("A project with this title already exists.");

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            var updatedProject = await _projectService.UpdateProjectAsync(id, inputModel, cancellationToken);
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
