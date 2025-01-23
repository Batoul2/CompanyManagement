using DotnetAPI.DTOs;
using DotnetAPI.InputModels;

namespace DotnetAPI.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> AddEmployeeAsync(EmployeeInputModel inputModel);
        Task<EmployeeDto> UpdateEmployeeAsync(int id, EmployeeInputModel inputModel);
        Task<bool> DeleteEmployeeAsync(int id);

        Task AssignProjectToEmployeeAsync(int employeeId, int projectId);
        Task RemoveProjectFromEmployeeAsync(int employeeId, int projectId);
    }

}
