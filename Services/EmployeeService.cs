using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.InputModels;
using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly CompanyDbContext _dbContext;
        private readonly IMapper _mapper;

        public EmployeeService(CompanyDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _dbContext.Employees
                .Include(e => e.CompanyEmployee)
                    .ThenInclude(ce => ce.Company)
                .Include(e => e.EmployeeProject)
                    .ThenInclude(ep => ep.Project)
                .ToListAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(int id)
        {
            var employee = await _dbContext.Employees
                .Include(e => e.CompanyEmployee)
                    .ThenInclude(ce => ce.Company)
                .Include(e => e.EmployeeProject)
                    .ThenInclude(ep => ep.Project)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto>  AddEmployeeAsync(EmployeeInputModel inputModel, CancellationToken cancellationToken)
        {
            var employee = _mapper.Map<Employee>(inputModel);

            // Handle Company Assignments
            var companyEmployees = inputModel.CompanyIds.Select(companyId => new CompanyEmployee
            {
                CompanyId = companyId,
                Employee = employee
            }).ToList();

            employee.CompanyEmployee = companyEmployees;

            // Handle Project Assignments
            var employeeProjects = inputModel.ProjectIds.Select(projectId => new EmployeeProject
            {
                ProjectId = projectId,
                Employee = employee
            }).ToList();

            employee.EmployeeProject = employeeProjects;

            await _dbContext.Employees.AddAsync(employee);
            await _dbContext.SaveChangesAsync(cancellationToken);
            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return employeeDto;
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(int id, EmployeeInputModel inputModel,CancellationToken cancellationToken)
        {
            var employee = await _dbContext.Employees
                .Include(e => e.CompanyEmployee)
                .Include(e => e.EmployeeProject)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            employee.FullName = inputModel.FullName;
            employee.Position = inputModel.Position;

            // Update Company Assignments
            employee.CompanyEmployee.Clear();
            employee.CompanyEmployee = inputModel.CompanyIds.Select(companyId => new CompanyEmployee
            {
                CompanyId = companyId,
                EmployeeId = id
            }).ToList();

            // Update Project Assignments
            employee.EmployeeProject.Clear();
            employee.EmployeeProject = inputModel.ProjectIds.Select(projectId => new EmployeeProject
            {
                ProjectId = projectId,
                EmployeeId = id
            }).ToList();

            await _dbContext.SaveChangesAsync(cancellationToken);
            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return employeeDto;
        }

        public async Task<bool> DeleteEmployeeAsync(int id, CancellationToken cancellationToken)
        {
            var employee = await _dbContext.Employees.FindAsync(id,cancellationToken);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            _dbContext.Employees.Remove(employee);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task AssignProjectToEmployeeAsync(int employeeId, int projectId, CancellationToken cancellationToken)
        {
            var employee = await _dbContext.Employees.FindAsync(employeeId);
            var project = await _dbContext.Projects.FindAsync(projectId);

            if (employee == null || project == null)
                throw new KeyNotFoundException("Employee or Project not found");

            var employeeProject = new EmployeeProject
            {
                EmployeeId = employeeId,
                ProjectId = projectId
            };

            _dbContext.EmployeeProject.Add(employeeProject);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveProjectFromEmployeeAsync(int employeeId, int projectId, CancellationToken cancellationToken)
        {
            var employeeProject = await _dbContext.EmployeeProject
                .FirstOrDefaultAsync(ep => ep.EmployeeId == employeeId && ep.ProjectId == projectId);

            if (employeeProject == null)
                throw new KeyNotFoundException("Employee-Project relationship not found");

            _dbContext.EmployeeProject.Remove(employeeProject);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
