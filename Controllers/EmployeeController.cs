using CompanyManagement.DTOs;
using CompanyManagement.InputModels;
using CompanyManagement.QueryParameters;
using CompanyManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace CompanyManagement.Controllers
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
            await _employeeService.AddEmployeeAsync(inputModel,cancellationToken);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = inputModel }, inputModel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int id,[FromBody] EmployeeInputModel inputModel,CancellationToken cancellationToken)
        {
            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, inputModel, cancellationToken);

            if (updatedEmployee==null)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Employee with ID {id} not found."
                });
            }

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

        // [HttpPost("{id}/upload-profile-picture")]
        // public async Task<IActionResult> UploadProfilePicture([FromRoute] int id, [FromForm] IFormFile profilePicture, CancellationToken cancellationToken)
        // {
        //     if (profilePicture == null || profilePicture.Length == 0)
        //     {
        //         return BadRequest("Profile picture is required.");
        //     }

        //     var employee = await _employeeService.GetEmployeeByIdAsync(id);
        //     if (employee == null)
        //     {
        //         return NotFound(new { Message = $"Employee with ID {id} not found." });
        //     }

        //     // Upload and update the profile picture
        //     var imagePath = await _employeeService.UploadProfilePictureAsync(id, profilePicture, cancellationToken);

        //     return Ok(new { Message = "Profile picture uploaded successfully", ImagePath = imagePath });
        // }

    }
}
