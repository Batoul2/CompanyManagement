using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CompanyManagement.Data;
using CompanyManagement.DTOs;
using CompanyManagement.InputModels;
using CompanyManagement.Models;
using CompanyManagement.Services;
using CompanyManagement.QueryParameters;

public class EmployeeServiceTests
{
    private readonly EmployeeService _employeeService;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CompanyDbContext _dbContext;

    public EmployeeServiceTests()
    {
        var options = new DbContextOptionsBuilder<CompanyDbContext>()
            .UseInMemoryDatabase("TestDatabase_Employees") // ✅ Use InMemoryDatabase
            .Options;

        _dbContext = new CompanyDbContext(options);
        _mapperMock = new Mock<IMapper>();
        _employeeService = new EmployeeService(_dbContext, _mapperMock.Object);

        _dbContext.Database.EnsureDeleted(); // ✅ Reset the database before each test run
        _dbContext.Database.EnsureCreated();

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _dbContext.Employees.RemoveRange(_dbContext.Employees);
        _dbContext.SaveChanges();

        _dbContext.Employees.Add(new Employee { Id = 1, FullName = "Alice Johnson", Position = "Developer" });
        _dbContext.Employees.Add(new Employee { Id = 2, FullName = "Bob Smith", Position = "Manager" });
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetAllEmployeesAsync_ShouldReturnListOfEmployees()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var queryParameters = new EmployeeQueryParameters { SearchTerm = null, SortBy = "fullname", SortDir = "asc", PageSize = 5 };

        _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeDto>>(It.IsAny<IEnumerable<Employee>>()))
            .Returns((IEnumerable<Employee> src) => src.Select(e => new EmployeeDto { Id = e.Id, FullName = e.FullName, Position = e.Position }));

        // Act
        var result = await _employeeService.GetAllEmployeesAsync(queryParameters, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Any()); // Ensure employees are returned
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_ShouldReturn_CorrectEmployee()
    {
        // Arrange
        int employeeId = 1;

        _mapperMock.Setup(m => m.Map<EmployeeDto>(It.IsAny<Employee>()))
            .Returns((Employee e) => new EmployeeDto { Id = e.Id, FullName = e.FullName, Position = e.Position });

        // Act
        var result = await _employeeService.GetEmployeeByIdAsync(employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employeeId, result.Id);
        Assert.Equal("Alice Johnson", result.FullName);
    }

    [Fact]
    public async Task AddEmployeeAsync_ShouldAddEmployeeToDatabase()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var inputModel = new EmployeeInputModel { FullName = "Charlie Brown", Position = "Analyst", CompanyIds = new List<int>(), ProjectIds = new List<int>() };
        var newEmployee = new Employee { Id = 3, FullName = inputModel.FullName, Position = inputModel.Position };

        _mapperMock.Setup(m => m.Map<Employee>(It.IsAny<EmployeeInputModel>()))
            .Returns(newEmployee);

        _mapperMock.Setup(m => m.Map<EmployeeDto>(It.IsAny<Employee>()))
            .Returns(new EmployeeDto { Id = newEmployee.Id, FullName = newEmployee.FullName, Position = newEmployee.Position });

        // Act
        var result = await _employeeService.AddEmployeeAsync(inputModel, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(inputModel.FullName, result.FullName);
        Assert.True(_dbContext.Employees.Any(e => e.FullName == inputModel.FullName));
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldUpdateExistingEmployee()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        int employeeId = 1;
        var updateModel = new EmployeeInputModel { FullName = "Alice Updated", Position = "Lead Developer", CompanyIds = new List<int>(), ProjectIds = new List<int>() };

        _mapperMock.Setup(m => m.Map(It.IsAny<EmployeeInputModel>(), It.IsAny<Employee>()))
            .Callback<EmployeeInputModel, Employee>((src, dest) => { dest.FullName = src.FullName; dest.Position = src.Position; });

        _mapperMock.Setup(m => m.Map<EmployeeDto>(It.IsAny<Employee>()))
            .Returns((Employee e) => new EmployeeDto { Id = e.Id, FullName = e.FullName, Position = e.Position });

        // Act
        var result = await _employeeService.UpdateEmployeeAsync(employeeId, updateModel, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateModel.FullName, result.FullName);
    }

    [Fact]
    public async Task DeleteEmployeeAsync_ShouldRemoveEmployeeFromDatabase()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        int employeeId = 1;

        // Act
        var result = await _employeeService.DeleteEmployeeAsync(employeeId, cancellationToken);

        // Assert
        Assert.True(result);
        Assert.False(_dbContext.Employees.Any(e => e.Id == employeeId));
    }
}
