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
        public async Task<IActionResult> GetAllCompanies([FromQuery] string sortBy = "Name", [FromQuery] string sortDirection = "asc")
        {
            var companies = await _companyService.GetAllCompaniesAsync(sortBy, sortDirection);
            return Ok(companies);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDto>> GetCompanyById(int id)
        {
            return Ok(await _companyService.GetCompanyByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddCompany(CompanyInputModel inputModel)
        {
            try
        {
            _logger.LogInformation("Attempting to add a new company: {CompanyName}", inputModel.Name);
            await _companyService.AddCompanyAsync(inputModel);
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
        public async Task<IActionResult> UpdateCompany(int id, CompanyInputModel inputModel)
        {
            await _companyService.UpdateCompanyAsync(id, inputModel);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            await _companyService.DeleteCompanyAsync(id);
            return NoContent();
        }
    }
}
