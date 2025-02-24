using CompanyManagement.DTOs;
using CompanyManagement.InputModels;
using CompanyManagement.QueryParameters;
using Microsoft.AspNetCore.Http.Metadata;

namespace CompanyManagement.Services
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
        Task<string> UploadProfilePictureAsync(int employeeId, IFormFile profilePicture, CancellationToken cancellationToken);
    }

}
