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

public class ProjectControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProjectControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllProjects_ShouldReturnListOfProjects()
    {
        // Act
        var response = await _client.GetAsync("/api/project");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var projects = JsonConvert.DeserializeObject<List<ProjectDto>>(content);

        projects.Should().NotBeNull();
        projects.Should().HaveCountGreaterThan(0); 
    }

    [Fact]
    public async Task GetProjectById_ShouldReturnProject_WhenExists()
    {
        // Arrange
        int projectId = 1; 

        // Act
        var response = await _client.GetAsync($"/api/project/{projectId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var project = JsonConvert.DeserializeObject<ProjectDto>(content);

        project.Should().NotBeNull();
        project.Id.Should().Be(projectId);
    }

    [Fact]
    public async Task AddProject_ShouldReturn_CreatedResponse()
    {
        // Arrange
        var newProject = new { Title = "AI Research", Duration = new TimeSpan(100, 0, 0, 0) };
        var content = new StringContent(JsonConvert.SerializeObject(newProject), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/project", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseBody = await response.Content.ReadAsStringAsync();
        var createdProject = JsonConvert.DeserializeObject<ProjectDto>(responseBody);

        createdProject.Should().NotBeNull();
        createdProject.Title.Should().Be("AI Research");
    }

    [Fact]
    public async Task UpdateProject_ShouldModifyExistingProject()
    {
        // Arrange
        int projectId = 1; 
        var updatedProject = new { Title = "Updated AI Research", Duration = new TimeSpan(180, 0, 0, 0) };
        var content = new StringContent(JsonConvert.SerializeObject(updatedProject), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/project/{projectId}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseBody = await response.Content.ReadAsStringAsync();
        var updatedResult = JsonConvert.DeserializeObject<ProjectDto>(responseBody);

        updatedResult.Should().NotBeNull();
        updatedResult.Title.Should().Be("Updated AI Research");
    }

    [Fact]
    public async Task DeleteProject_ShouldRemoveProject()
    {
        // Arrange
        int projectId = 2;

        // Act
        var response = await _client.DeleteAsync($"/api/project/{projectId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/project/{projectId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
