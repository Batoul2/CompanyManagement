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

public class CompanyServiceTests
{
    private readonly CompanyService _companyService;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CompanyDbContext _dbContext;

    public CompanyServiceTests()
    {
        var options = new DbContextOptionsBuilder<CompanyDbContext>()
            .UseInMemoryDatabase("TestDatabase") 
            .Options;

        _dbContext = new CompanyDbContext(options);
        _mapperMock = new Mock<IMapper>();
        _companyService = new CompanyService(_dbContext, _mapperMock.Object);

        _dbContext.Database.EnsureDeleted(); 
        _dbContext.Database.EnsureCreated();
        // Seed test data
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _dbContext.Companies.RemoveRange(_dbContext.Companies);
        _dbContext.SaveChanges();

        _dbContext.Companies.Add(new Company { Id = 1, Name = "TechCorp" });
        _dbContext.Companies.Add(new Company { Id = 2, Name = "Innovate Ltd" });
        _dbContext.SaveChanges();
    }


    [Fact]
    public async Task GetAllCompaniesAsync_ShouldReturnFilteredSortedPagedCompanies()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var queryParameters = new CompanyQueryParameters
        {
            SearchTerm = null,
            SortBy = "name",
            SortDir = "asc",
            PageSize = 5
        };

        _mapperMock.Setup(m => m.Map<IEnumerable<CompanyDto>>(It.IsAny<IEnumerable<Company>>()))
            .Returns((IEnumerable<Company> src) => src.Select(c => new CompanyDto { Id = c.Id, Name = c.Name }));

        // Act
        var result = await _companyService.GetAllCompaniesAsync(queryParameters, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Any());
    }

    [Fact]
    public async Task GetCompanyByIdAsync_ShouldReturn_CorrectCompany()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        int companyId = 1;

        _mapperMock.Setup(m => m.Map<CompanyDto>(It.IsAny<Company>()))
            .Returns((Company c) => new CompanyDto { Id = c.Id, Name = c.Name });

        // Act
        var result = await _companyService.GetCompanyByIdAsync(companyId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(companyId, result.Id);
        Assert.Equal("TechCorp", result.Name);
    }

    [Fact]
    public async Task AddCompanyAsync_ShouldAddCompanyToDatabase()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var inputModel = new CompanyInputModel { Name = "NewTech Solutions" };
        var newCompany = new Company { Id = 3, Name = inputModel.Name };

        _mapperMock.Setup(m => m.Map<Company>(It.IsAny<CompanyInputModel>()))
            .Returns(newCompany);

        _mapperMock.Setup(m => m.Map<CompanyDto>(It.IsAny<Company>()))
            .Returns(new CompanyDto { Id = newCompany.Id, Name = newCompany.Name });

        // Act
        var result = await _companyService.AddCompanyAsync(inputModel, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(inputModel.Name, result.Name);
        Assert.True(_dbContext.Companies.Any(c => c.Name == inputModel.Name));
    }

    [Fact]
    public async Task UpdateCompanyAsync_ShouldUpdateExistingCompany()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        int companyId = 1;
        var updateModel = new CompanyInputModel { Name = "UpdatedTechCorp" };

        _mapperMock.Setup(m => m.Map(It.IsAny<CompanyInputModel>(), It.IsAny<Company>()))
            .Callback<CompanyInputModel, Company>((src, dest) => dest.Name = src.Name);

        _mapperMock.Setup(m => m.Map<CompanyDto>(It.IsAny<Company>()))
            .Returns((Company c) => new CompanyDto { Id = c.Id, Name = c.Name });

        // Act
        var result = await _companyService.UpdateCompanyAsync(companyId, updateModel, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateModel.Name, result.Name);
    }

    [Fact]
    public async Task DeleteCompanyAsync_ShouldRemoveCompanyFromDatabase()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        int companyId = 1;

        // Act
        var result = await _companyService.DeleteCompanyAsync(companyId, cancellationToken);

        // Assert
        Assert.True(result);
        Assert.False(_dbContext.Companies.Any(c => c.Id == companyId));
    }
}
