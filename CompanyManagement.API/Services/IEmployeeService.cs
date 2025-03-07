using CompanyManagement.API.DTOs;
using CompanyManagement.API.InputModels;
using CompanyManagement.API.QueryParameters;
using Microsoft.AspNetCore.Http.Metadata;

namespace CompanyManagement.API.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(EmployeeQueryParameters parameters, CancellationToken cancellationToken);
        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> AddEmployeeAsync(EmployeeInputModel inputModel,CancellationToken cancellationToken);
        Task<EmployeeDto?> UpdateEmployeeAsync(int id, EmployeeInputModel inputModel, CancellationToken cancellationToken);
        Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken);

        Task AssignProjectToEmployeeAsync(int employeeId, int projectId, CancellationToken cancellationToken);
        Task<bool> RemoveProjectFromEmployeeAsync(int employeeId, int projectId, CancellationToken cancellationToken); 
        Task<bool> EmployeeNameExistsAsync(string name, int excludeId, CancellationToken cancellationToken);
        Task<byte[]> GenerateEmployeeReportAsync();
        Task<byte[]> GenerateEmployeeExcelReportAsync();
    }

}
