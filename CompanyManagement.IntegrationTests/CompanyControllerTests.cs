using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using CompanyManagement.DTOs;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;

public class CompanyControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CompanyControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllCompanies_ShouldReturnListOfCompanies()
    {
        // Act
        var response = await _client.GetAsync("/api/company");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var companies = JsonConvert.DeserializeObject<List<CompanyDto>>(content);

        companies.Should().NotBeNull();
        companies.Should().HaveCountGreaterThan(0); 
    }

    [Fact]
    public async Task GetCompanyById_ShouldReturnCompany_WhenExists()
    {
        // Arrange
        int companyId = 1; 

        // Act
        var response = await _client.GetAsync($"/api/company/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var company = JsonConvert.DeserializeObject<CompanyDto>(content);

        company.Should().NotBeNull();
        company.Id.Should().Be(companyId);
    }

    [Fact]
    public async Task AddCompany_ShouldReturn_CreatedResponse()
    {
        // Arrange
        var newCompany = new { Name = "NewTech Solutions" };
        var content = new StringContent(JsonConvert.SerializeObject(newCompany), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/company", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseBody = await response.Content.ReadAsStringAsync();
        var createdCompany = JsonConvert.DeserializeObject<CompanyDto>(responseBody);

        createdCompany.Should().NotBeNull();
        createdCompany.Name.Should().Be("NewTech Solutions");
    }

    [Fact]
    public async Task UpdateCompany_ShouldModifyExistingCompany()
    {
        // Arrange
        int companyId = 12; 
        var updatedCompany = new { Name = "UpdatedTechCorp" };
        var content = new StringContent(JsonConvert.SerializeObject(updatedCompany), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/company/{companyId}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadAsStringAsync();
        var updatedResult = JsonConvert.DeserializeObject<CompanyDto>(responseBody);

        updatedResult.Should().NotBeNull();
        updatedResult.Name.Should().Be("UpdatedTechCorp");
    }

    [Fact]
    public async Task DeleteCompany_ShouldRemoveCompany()
    {
        // Arrange
        int companyId =11; 

        // Act
        var response = await _client.DeleteAsync($"/api/company/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/company/{companyId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
