using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using CompanyManagement.DTOs;
using FluentAssertions;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.TestHost;

public class EmployeeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EmployeeControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllEmployees_ShouldReturnListOfEmployees()
    {
        // Act
        var response = await _client.GetAsync("/api/employee");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var employees = JsonConvert.DeserializeObject<List<EmployeeDto>>(content);

        employees.Should().NotBeNull();
        employees.Should().HaveCountGreaterThan(0); 
    }

    [Fact]
    public async Task GetEmployeeById_ShouldReturnEmployee_WhenExists()
    {
        // Arrange
        int employeeId = 1; 

        // Act
        var response = await _client.GetAsync($"/api/employee/{employeeId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var employee = JsonConvert.DeserializeObject<EmployeeDto>(content);

        employee.Should().NotBeNull();
        employee.Id.Should().Be(employeeId);
    }

    [Fact]
    public async Task AddEmployee_ShouldReturn_CreatedResponse()
    {
        // Arrange
        var newEmployee = new { FullName = "John Doe", Position = "Software Engineer", CompanyIds = new List<int>(1), ProjectIds = new List<int>(1) };
        var content = new StringContent(JsonConvert.SerializeObject(newEmployee), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/employee", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseBody = await response.Content.ReadAsStringAsync();
        var createdEmployee = JsonConvert.DeserializeObject<EmployeeDto>(responseBody);

        createdEmployee.Should().NotBeNull();
        createdEmployee.FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task UpdateEmployee_ShouldModifyExistingEmployee()
    {
        // Arrange
        int employeeId = 1; 
        var updatedEmployee = new { FullName = "Jane Smith", Position = "Lead Developer", CompanyIds = new List<int>(), ProjectIds = new List<int>() };
        var content = new StringContent(JsonConvert.SerializeObject(updatedEmployee), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/employee/{employeeId}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadAsStringAsync();
        var updatedResult = JsonConvert.DeserializeObject<EmployeeDto>(responseBody);

        updatedResult.Should().NotBeNull();
        updatedResult.FullName.Should().Be("Jane Smith");
    }

    [Fact]
    public async Task DeleteEmployee_ShouldRemoveEmployee()
    {
        // Arrange
        int employeeId = 2; 

        // Act
        var response = await _client.DeleteAsync($"/api/employee/{employeeId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/employee/{employeeId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
