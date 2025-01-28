using DotnetAPI.DTOs;
using DotnetAPI.InputModels;
using DotnetAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllEmployees()
        {
            return Ok(await _employeeService.GetAllEmployeesAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeById([FromRoute]int id)
        {
            return Ok(await _employeeService.GetEmployeeByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeInputModel inputModel,CancellationToken cancellationToken)
        {
            await _employeeService.AddEmployeeAsync(inputModel,cancellationToken);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = inputModel }, inputModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int id,[FromBody] EmployeeInputModel inputModel,CancellationToken cancellationToken)
        {
            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, inputModel, cancellationToken);

            return Ok(updatedEmployee);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id,CancellationToken cancellationToken)
        {
            var success = await _employeeService.DeleteEmployeeAsync(id, cancellationToken);

            if (!success)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            return Ok($"Employee with ID {id} successfully deleted.");
        }

        [HttpPost("{employeeId}/projects/{projectId}")]
        public async Task<IActionResult> AssignProjectToEmployee([FromRoute] int employeeId,[FromRoute] int projectId,CancellationToken cancellationToken)
        {
            await _employeeService.AssignProjectToEmployeeAsync(employeeId, projectId,cancellationToken);
            return Ok("Project assigned successfully");
        }

        [HttpDelete("{employeeId}/projects/{projectId}")]
        public async Task<IActionResult> RemoveProjectFromEmployee([FromRoute] int employeeId,[FromRoute] int projectId,CancellationToken cancellationToken)
        {
            var success = await _employeeService.RemoveProjectFromEmployeeAsync(employeeId, projectId, cancellationToken);

            if (!success)
            {
                return NotFound($"Employee-Project relationship not found for Employee ID {employeeId} and Project ID {projectId}.");
            }

            return Ok("Project removed successfully from employee.");
        }
    }
}
