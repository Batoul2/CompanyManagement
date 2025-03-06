using CompanyManagement.API.DTOs;
using CompanyManagement.API.InputModels;
using CompanyManagement.API.QueryParameters;
using CompanyManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CompanyManagement.Data.Models;

namespace CompanyManagement.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ICompanyService companyService,ILogger<CompanyController> logger)
        {
            _companyService = companyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanies([FromQuery] CompanyQueryParameters parameters, CancellationToken cancellationToken)
        {
            var companies = await _companyService.GetAllCompaniesAsync(parameters, cancellationToken);
            return Ok(companies);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDto>> GetCompanyById([FromRoute]int id,CancellationToken cancellationToken)
        {
            var company = await _companyService.GetCompanyByIdAsync(id, cancellationToken);

            if (company == null)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Company with ID {id} not found."
                });
            }

            return Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> AddCompany([FromBody] CompanyInputModel inputModel, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            var nameExists = await _companyService.CompanyNameExistsAsync(inputModel.Name, 0, cancellationToken);
            if (nameExists)
            {
                errors.Add("A company with this name already exists.");
            }

            if (errors.Any())
            {
                return BadRequest(new { Errors = errors });
            }

            var createdCompany = await _companyService.AddCompanyAsync(inputModel, cancellationToken);
            
            return CreatedAtAction(nameof(GetCompanyById), new { id = createdCompany.Id }, createdCompany);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany([FromRoute] int id, [FromBody] CompanyInputModel inputModel, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            var existingCompany = await _companyService.GetCompanyByIdAsync(id, cancellationToken);
            if (existingCompany == null)
            {
                errors.Add($"Company with ID {id} not found.");
            }

            var nameExists = await _companyService.CompanyNameExistsAsync(inputModel.Name, id, cancellationToken);
            if (nameExists)
            {
                errors.Add("A company with this name already exists.");
            }

            if (errors.Any())
            {
                return BadRequest(new { Errors = errors });
            }

            var updatedCompany = await _companyService.UpdateCompanyAsync(id, inputModel, cancellationToken);
            
            return Ok(updatedCompany);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany([FromRoute]int id,CancellationToken cancellationToken)
        {
            var success = await _companyService.DeleteCompanyAsync(id, cancellationToken);

             if (!success)
            {
                return NotFound(new
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Company with ID {id} not found."
                });
            }

            return NoContent();
        }
    }
}
