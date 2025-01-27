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
        public async Task<ActionResult<EmployeeDto>> GetEmployeeById(int id)
        {
            return Ok(await _employeeService.GetEmployeeByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(EmployeeInputModel inputModel,CancellationToken cancellationToken)
        {
            await _employeeService.AddEmployeeAsync(inputModel,cancellationToken);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = inputModel }, inputModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeInputModel inputModel,CancellationToken cancellationToken)
        {
            await _employeeService.UpdateEmployeeAsync(id, inputModel,cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id,CancellationToken cancellationToken)
        {
            await _employeeService.DeleteEmployeeAsync(id,cancellationToken);
            return NoContent();
        }

        [HttpPost("{employeeId}/projects/{projectId}")]
        public async Task<IActionResult> AssignProjectToEmployee(int employeeId, int projectId,CancellationToken cancellationToken)
        {
            await _employeeService.AssignProjectToEmployeeAsync(employeeId, projectId,cancellationToken);
            return Ok("Project assigned successfully");
        }

        [HttpDelete("{employeeId}/projects/{projectId}")]
        public async Task<IActionResult> RemoveProjectFromEmployee(int employeeId, int projectId,CancellationToken cancellationToken)
        {
            await _employeeService.RemoveProjectFromEmployeeAsync(employeeId, projectId,cancellationToken);
            return Ok("Project removed successfully");
        }
    }
}
