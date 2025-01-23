using DotnetAPI.DTOs;
using DotnetAPI.InputModels;

namespace DotnetAPI.Services
{
    public interface ICompanyService
    {
        Task<CompanyDto> AddCompanyAsync(CompanyInputModel inputModel);
        Task<CompanyDto> GetCompanyByIdAsync(int id);
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(string sortBy = "Name", string sortDirection = "asc");
        Task<CompanyDto> UpdateCompanyAsync(int id, CompanyInputModel inputModel);
        Task<bool> DeleteCompanyAsync(int id);
    }
}
