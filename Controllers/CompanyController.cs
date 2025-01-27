using DotnetAPI.DTOs;
using DotnetAPI.InputModels;
using DotnetAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
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
        public async Task<IActionResult> GetAllCompanies(CancellationToken cancellationToken,[FromQuery] string sortBy = "Name", [FromQuery] string sortDirection = "asc")
        {
            var companies = await _companyService.GetAllCompaniesAsync(cancellationToken,sortBy, sortDirection);
            return Ok(companies);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDto>> GetCompanyById(int id,CancellationToken cancellationToken)
        {
            return Ok(await _companyService.GetCompanyByIdAsync(id,cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> AddCompany(CompanyInputModel inputModel,CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to add a new company: {CompanyName}", inputModel.Name);
                await _companyService.AddCompanyAsync(inputModel,cancellationToken);
                _logger.LogInformation("Successfully added company: {CompanyName}", inputModel.Name);
                return CreatedAtAction(nameof(GetCompanyById), new { id = inputModel }, inputModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding company: {CompanyName}", inputModel.Name);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, CompanyInputModel inputModel,CancellationToken cancellationToken)
        {
            await _companyService.UpdateCompanyAsync(id, inputModel,cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id,CancellationToken cancellationToken)
        {
            await _companyService.DeleteCompanyAsync(id,cancellationToken);
            return NoContent();
        }
    }
}
