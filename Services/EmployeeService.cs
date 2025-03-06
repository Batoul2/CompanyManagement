using AutoMapper;
using CompanyManagement.Data;
using CompanyManagement.DTOs;
using CompanyManagement.InputModels;
using CompanyManagement.Models;
using CompanyManagement.QueryParameters;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CompanyManagement.Services
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

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(EmployeeQueryParameters parameters, CancellationToken cancellationToken)
        {
            var query = _dbContext.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                query = query.Where(e => e.FullName.Contains(parameters.SearchTerm));
            }

            query = parameters.SortBy.ToLower() switch
            {
                "fullname" => parameters.SortDir.ToLower() == "desc" ? query.OrderByDescending(e => e.FullName) : query.OrderBy(e => e.FullName),
                "id" => parameters.SortDir.ToLower() == "desc" ? query.OrderByDescending(e => e.Id) : query.OrderBy(e => e.Id),
                _ => query.OrderBy(e => e.FullName)
            };

            var employees = await query
                .Skip(parameters.GetSkipAmount())
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _dbContext.Employees
                .Include(e => e.CompanyEmployee)
                    .ThenInclude(ce => ce.Company)
                .Include(e => e.EmployeeProject)
                    .ThenInclude(ep => ep.Project)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return null;

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

        public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, EmployeeInputModel inputModel,CancellationToken cancellationToken)
        {
            var employee = await _dbContext.Employees
                .Include(e => e.CompanyEmployee)
                .Include(e => e.EmployeeProject)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return null;

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
                return false;

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

        public async Task<bool> RemoveProjectFromEmployeeAsync(int employeeId, int projectId, CancellationToken cancellationToken)
        {
            var employeeProject = await _dbContext.EmployeeProject
                .FirstOrDefaultAsync(ep => ep.EmployeeId == employeeId && ep.ProjectId == projectId);

            if (employeeProject == null)
                throw new KeyNotFoundException("Employee-Project relationship not found");

            _dbContext.EmployeeProject.Remove(employeeProject);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> EmployeeNameExistsAsync(string name, int excludeId, CancellationToken cancellationToken)
        {
            return await _dbContext.Employees.AnyAsync(e => e.FullName == name && e.Id != excludeId, cancellationToken);
        }

        public async Task<byte[]> GenerateEmployeeReportAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var employees = await _dbContext.Employees
                .Include(e => e.CompanyEmployee)
                    .ThenInclude(ce => ce.Company)
                .Include(e => e.EmployeeProject)
                    .ThenInclude(ep => ep.Project)
                .ToListAsync();

            byte[] pdfData = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    
                    page.Header()
                        .Text("Employee Report")
                        .Bold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content().Column(column =>
                    {
                        foreach (var employee in employees)
                        {
                            column.Item().Text($"Employee: {employee.FullName}").Bold().FontSize(16);
                            column.Item().Text($"Position: {employee.Position}");

                            // Companies Section
                            column.Item().Text("Companies:").Underline().FontSize(14);
                            if (employee.CompanyEmployee.Any())
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("Company Name").Bold();
                                    });

                                    foreach (var ce in employee.CompanyEmployee)
                                    {
                                        table.Cell().Text(ce.Company.Name);
                                    }
                                });
                            }
                            else
                            {
                                column.Item().Text("No companies assigned.");
                            }

                            // Projects Section
                            column.Item().Text("Projects:").Underline().FontSize(14);
                            if (employee.EmployeeProject.Any())
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("Project Name").Bold();
                                    });

                                    foreach (var ep in employee.EmployeeProject)
                                    {
                                        table.Cell().Text(ep.Project.Title);
                                    }
                                });
                            }
                            else
                            {
                                column.Item().Text("No projects assigned.");
                            }

                            column.Item().LineHorizontal(1);
                        }
                    });

                    page.Footer().AlignCenter().Text("Generated by Company Management System");
                });
            }).GeneratePdf();

            return pdfData;
        }
    }
}
