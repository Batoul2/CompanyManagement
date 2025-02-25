using CompanyManagement.DTOs;
using CompanyManagement.InputModels;
using CompanyManagement.QueryParameters;

namespace CompanyManagement.Services
{
    public interface ICompanyService
    {
        Task<CompanyDto> AddCompanyAsync(CompanyInputModel inputModel, CancellationToken cancellationToken);
        Task<CompanyDto?> GetCompanyByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(CompanyQueryParameters parameters, CancellationToken cancellationToken);
        Task<CompanyDto?> UpdateCompanyAsync(int id, CompanyInputModel inputModel, CancellationToken cancellationToken);
        Task<bool> DeleteCompanyAsync(int id, CancellationToken cancellationToken);
        Task<bool> CompanyNameExistsAsync(string name, int excludeId, CancellationToken cancellationToken);
    }
}
