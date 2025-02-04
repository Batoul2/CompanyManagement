using CompanyManagement.DTOs;
using CompanyManagement.InputModels;

namespace CompanyManagement.Services
{
    public interface ICompanyService
    {
        Task<CompanyDto> AddCompanyAsync(CompanyInputModel inputModel, CancellationToken cancellationToken);
        Task<CompanyDto> GetCompanyByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(CancellationToken cancellationToken, string sortBy = "Name", string sortDirection = "asc");
        Task<CompanyDto> UpdateCompanyAsync(int id, CompanyInputModel inputModel, CancellationToken cancellationToken);
        Task<bool> DeleteCompanyAsync(int id, CancellationToken cancellationToken);
    }
}
