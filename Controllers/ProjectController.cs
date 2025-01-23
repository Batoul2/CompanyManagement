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
        public async Task<IActionResult> GetFilteredProjects([FromQuery] string? title)
        {
            var projects = await _projectService.GetFilteredProjectsAsync(title);

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
        public async Task<IActionResult> AddProject(ProjectInputModel inputModel)
        {
            await _projectService.AddProjectAsync(inputModel);
            return CreatedAtAction(nameof(GetProjectById), new { id = inputModel }, inputModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, ProjectInputModel inputModel)
        {
            await _projectService.UpdateProjectAsync(id, inputModel);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await _projectService.DeleteProjectAsync(id);
            return NoContent();
        }

    }
}
