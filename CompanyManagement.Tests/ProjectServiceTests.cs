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

public class ProjectServiceTests
{
    private readonly ProjectService _projectService;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CompanyDbContext _dbContext;

    public ProjectServiceTests()
    {
        var options = new DbContextOptionsBuilder<CompanyDbContext>()
            .UseInMemoryDatabase("TestDatabase_Projects") 
            .Options;

        _dbContext = new CompanyDbContext(options);
        _mapperMock = new Mock<IMapper>();
        _projectService = new ProjectService(_dbContext, _mapperMock.Object);

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _dbContext.Projects.RemoveRange(_dbContext.Projects);
        _dbContext.SaveChanges();

        _dbContext.Projects.Add(new Project { Id = 1, Title = "AI Research", Duration = new TimeSpan(90, 0, 0, 0) });
        _dbContext.Projects.Add(new Project { Id = 2, Title = "Web App Development", Duration = new TimeSpan(120, 0, 0, 0) });
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetAllProjectsAsync_ShouldReturnListOfProjects()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var queryParameters = new ProjectQueryParameters { SearchTerm = null, SortBy = "title", SortDir = "asc", PageSize = 5 };

        _mapperMock.Setup(m => m.Map<IEnumerable<ProjectDto>>(It.IsAny<IEnumerable<Project>>()))
            .Returns((IEnumerable<Project> src) => src.Select(p => new ProjectDto { Id = p.Id, Title = p.Title, Duration = p.Duration }));

        // Act
        var result = await _projectService.GetFilteredProjectsAsync(queryParameters, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Any()); // Ensure projects are returned
    }

    [Fact]
    public async Task GetProjectByIdAsync_ShouldReturn_CorrectProject()
    {
        // Arrange
        int projectId = 1;

        _mapperMock.Setup(m => m.Map<ProjectDto>(It.IsAny<Project>()))
            .Returns((Project p) => new ProjectDto { Id = p.Id, Title = p.Title, Duration = p.Duration });

        // Act
        var result = await _projectService.GetProjectByIdAsync(projectId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(projectId, result.Id);
        Assert.Equal("AI Research", result.Title);
    }

    [Fact]
    public async Task AddProjectAsync_ShouldAddProjectToDatabase()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var inputModel = new ProjectInputModel { Title = "New Blockchain Project", Duration = new TimeSpan(150, 0, 0, 0) };
        var newProject = new Project { Id = 3, Title = inputModel.Title, Duration = inputModel.Duration };

        _mapperMock.Setup(m => m.Map<Project>(It.IsAny<ProjectInputModel>()))
            .Returns(newProject);

        _mapperMock.Setup(m => m.Map<ProjectDto>(It.IsAny<Project>()))
            .Returns(new ProjectDto { Id = newProject.Id, Title = newProject.Title, Duration = newProject.Duration });

        // Act
        var result = await _projectService.AddProjectAsync(inputModel, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(inputModel.Title, result.Title);
        Assert.True(_dbContext.Projects.Any(p => p.Title == inputModel.Title));
    }

    [Fact]
    public async Task UpdateProjectAsync_ShouldUpdateExistingProject()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        int projectId = 1;
        var updateModel = new ProjectInputModel { Title = "Updated AI Research", Duration = new TimeSpan(180, 0, 0, 0) };

        _mapperMock.Setup(m => m.Map(It.IsAny<ProjectInputModel>(), It.IsAny<Project>()))
            .Callback<ProjectInputModel, Project>((src, dest) => { dest.Title = src.Title; dest.Duration = src.Duration; });

        _mapperMock.Setup(m => m.Map<ProjectDto>(It.IsAny<Project>()))
            .Returns((Project p) => new ProjectDto { Id = p.Id, Title = p.Title, Duration = p.Duration });

        // Act
        var result = await _projectService.UpdateProjectAsync(projectId, updateModel, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateModel.Title, result.Title);
    }


    [Fact]
    public async Task DeleteProjectAsync_ShouldRemoveProjectFromDatabase()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        int projectId = 1;

        // Act
        var result = await _projectService.DeleteProjectAsync(projectId, cancellationToken);

        // Assert
        Assert.True(result);
        Assert.False(_dbContext.Projects.Any(p => p.Id == projectId));
    }
}
