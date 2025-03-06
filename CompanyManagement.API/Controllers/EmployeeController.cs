using CompanyManagement.API.DTOs;
using CompanyManagement.API.InputModels;
using CompanyManagement.API.QueryParameters;
using CompanyManagement.API.Services;
using Microsoft.AspNetCore.Mvc;
using CompanyManagement.Data.Models;

namespace CompanyManagement.API.Controllers
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
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAllEmployees([FromQuery] EmployeeQueryParameters parameters, CancellationToken cancellationToken)
        {
            return Ok(await _employeeService.GetAllEmployeesAsync(parameters, cancellationToken));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeById([FromRoute]int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Employee with ID {id} not found."
                });
            }

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeInputModel inputModel,CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            var nameExists = await _employeeService.EmployeeNameExistsAsync(inputModel.FullName, 0, cancellationToken);
            if (nameExists)
                errors.Add("An employee with this name already exists.");

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            await _employeeService.AddEmployeeAsync(inputModel,cancellationToken);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = inputModel }, inputModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int id, [FromBody] EmployeeInputModel inputModel, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            var existingEmployee = await _employeeService.GetEmployeeByIdAsync(id);
            if (existingEmployee == null)
                errors.Add($"Employee with ID {id} not found.");

            var nameExists = await _employeeService.EmployeeNameExistsAsync(inputModel.FullName, id, cancellationToken);
            if (nameExists)
                errors.Add("An employee with this name already exists.");

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, inputModel, cancellationToken);
            return Ok(updatedEmployee);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id,CancellationToken cancellationToken)
        {
            var success = await _employeeService.DeleteEmployeeAsync(id, cancellationToken);

            if (!success)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Employee with ID {id} not found."
                });
            }

            return NoContent();
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
        [HttpGet("generate-report")]
        public async Task<IActionResult> GenerateEmployeeReport()
        {
            var pdfData = await _employeeService.GenerateEmployeeReportAsync();

            if (pdfData == null || pdfData.Length == 0)
            {
                return StatusCode(500, "Failed to generate PDF");
            }

            return File(pdfData, "application/pdf", "EmployeeReport.pdf");
        }

    }
}
