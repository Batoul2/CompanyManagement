using CompanyManagement.DTOs;
using CompanyManagement.InputModels;

namespace CompanyManagement.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> AddEmployeeAsync(EmployeeInputModel inputModel,CancellationToken cancellationToken);
        Task<EmployeeDto> UpdateEmployeeAsync(int id, EmployeeInputModel inputModel, CancellationToken cancellationToken);
        Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken);

        Task AssignProjectToEmployeeAsync(int employeeId, int projectId, CancellationToken cancellationToken);
        Task<bool> RemoveProjectFromEmployeeAsync(int employeeId, int projectId, CancellationToken cancellationToken);
    }

}
