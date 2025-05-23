using AutoMapper;
using CompanyManagement.Data.Data;
using CompanyManagement.API.DTOs;
using CompanyManagement.API.InputModels;
using CompanyManagement.Data.Models;
using CompanyManagement.Data.JunctionModels;
using CompanyManagement.API.QueryParameters;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClosedXML.Excel;

namespace CompanyManagement.API.Services
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

            var companies = await _dbContext.Companies
                .Include(c => c.CompanyEmployee)
                    .ThenInclude(ce => ce.Employee)
                        .ThenInclude(e => e.EmployeeProject)
                            .ThenInclude(ep => ep.Project)
                .ToListAsync();

            byte[] pdfData = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(50);

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Report").Bold().FontSize(24).FontColor(Colors.Blue.Medium);
                        });

                        row.ConstantItem(80).Image("C:\\Users\\hp\\Downloads\\logo.jpg"); 
                    });

                    page.Content().Column(column =>
                    {
                        foreach (var company in companies)
                        {
                            column.Item().Text($"Company: {company.Name}").Bold().FontSize(18).FontColor(Colors.Black);
                            column.Item().LineHorizontal(1); 

                            if (company.CompanyEmployee.Any())
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3); 
                                        columns.RelativeColumn(2); 
                                        columns.RelativeColumn(5); 
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("Employee Name").Bold();
                                        header.Cell().Text("Position").Bold();
                                        header.Cell().Text("Projects").Bold();
                                    });

                                    foreach (var ce in company.CompanyEmployee)
                                    {
                                        var employee = ce.Employee;
                                        var projects = employee.EmployeeProject.Select(ep => ep.Project.Title).ToList();
                                        string projectList = projects.Any() ? string.Join(", ", projects) : "No projects assigned";

                                        table.Cell().Text(employee.FullName);
                                        table.Cell().Text(employee.Position);
                                        table.Cell().Text(projectList);
                                    }
                                });
                            }
                            else
                            {
                                column.Item().Text("No employees assigned.").Italic();
                            }
                            column.Item().PaddingBottom(20);
                            column.Item().LineHorizontal(2);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });

                });
            }).GeneratePdf();

            return pdfData;
        }

        public async Task<byte[]> GenerateEmployeeExcelReportAsync()
        {
            var companies = await _dbContext.Companies
                .Include(c => c.CompanyEmployee)
                    .ThenInclude(ce => ce.Employee)
                        .ThenInclude(e => e.EmployeeProject)
                            .ThenInclude(ep => ep.Project)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Employee Report");

            int row = 1;

            worksheet.AddPicture(@"C:\Users\hp\Downloads\logo.jpg")
                .MoveTo(worksheet.Cell(row, 5)) 
                .Scale(0.5);

            row += 2;

            worksheet.Cell(row, 1).Value = "Company Employee Report";
            worksheet.Range(row, 1, row, 4).Merge().Style.Font.Bold = true;
            row += 2;

            foreach (var company in companies)
            {
                worksheet.Cell(row, 1).Value = $"Company: {company.Name}";
                worksheet.Range(row, 1, row, 4).Style.Font.Bold = true;
                row++;

                worksheet.Cell(row, 1).Value = "Employee Name";
                worksheet.Cell(row, 2).Value = "Position";
                worksheet.Cell(row, 3).Value = "Projects";

                worksheet.Range(row, 1, row, 3).Style.Font.Bold = true;
                worksheet.Range(row, 1, row, 3).Style.Fill.BackgroundColor = XLColor.LightGray;
                int tableStartRow = row;
                row++;

                if (company.CompanyEmployee.Any())
                {
                    foreach (var ce in company.CompanyEmployee)
                    {
                        var employee = ce.Employee;
                        var projects = employee.EmployeeProject.Select(ep => ep.Project.Title).ToList();
                        string projectList = projects.Any() ? string.Join(", ", projects) : "No projects assigned";

                        worksheet.Cell(row, 1).Value = employee.FullName;
                        worksheet.Cell(row, 2).Value = employee.Position;
                        worksheet.Cell(row, 3).Value = projectList;
                        row++;
                    }
                    var tableRange = worksheet.Range($"A{tableStartRow}:C{row - 1}");
                    tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                    tableRange.Style.Border.OutsideBorderColor = XLColor.Black;
                    tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    tableRange.Style.Border.InsideBorderColor = XLColor.Gray;

                    row++;
                }
                else
                {
                    worksheet.Cell(row, 1).Value = "No employees assigned.";
                    worksheet.Range(row, 1, row, 3).Merge();
                    row++;
                }
                row++;
            }

            worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
