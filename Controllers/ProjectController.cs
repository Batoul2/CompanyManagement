using DotnetAPI.DTOs;
using DotnetAPI.InputModels;
using DotnetAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
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
        public async Task<IActionResult> GetFilteredProjects([FromQuery] string? title,CancellationToken cancellationToken)
        {
            var projects = await _projectService.GetFilteredProjectsAsync(title,cancellationToken);

            if (projects == null || !projects.Any())
            {
                return NotFound("No projects found matching the criteria.");
            }

            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
        {
            return Ok(await _projectService.GetProjectByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddProject(ProjectInputModel inputModel,CancellationToken cancellationToken)
        {
            await _projectService.AddProjectAsync(inputModel,cancellationToken);
            return CreatedAtAction(nameof(GetProjectById), new { id = inputModel }, inputModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, ProjectInputModel inputModel,CancellationToken cancellationToken)
        {
            await _projectService.UpdateProjectAsync(id, inputModel,cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id,CancellationToken cancellationToken)
        {
            await _projectService.DeleteProjectAsync(id,cancellationToken);
            return NoContent();
        }

    }
}
