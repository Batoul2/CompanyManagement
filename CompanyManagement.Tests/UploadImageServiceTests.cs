using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using CompanyManagement.Data;
using CompanyManagement.Services;
using CompanyManagement.DTOs;
using CompanyManagement.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;

public class UploadImageServiceTests
{
    private readonly UploadImageService _uploadImageService;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly CompanyDbContext _dbContext;

    public UploadImageServiceTests()
    {
        var options = new DbContextOptionsBuilder<CompanyDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        _dbContext = new CompanyDbContext(options);
        _fileServiceMock = new Mock<IFileService>();

        _uploadImageService = new UploadImageService(_dbContext, _fileServiceMock.Object);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _dbContext.Employees.RemoveRange(_dbContext.Employees);
        _dbContext.SaveChanges();

        _dbContext.Employees.Add(new Employee { Id = 1, FullName = "John Doe" });
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task UploadImageAsync_ShouldSaveImageAndReturnImageDto()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        var memoryStream = new MemoryStream();
        mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);
        mockFile.Setup(f => f.Length).Returns(1024); // 1KB
        mockFile.Setup(f => f.FileName).Returns("test.jpg");

        _fileServiceMock.Setup(s => s.SaveFileAsync(mockFile.Object))
            .ReturnsAsync("saved-test.jpg");

        // Act
        var result = await _uploadImageService.UploadImageAsync(1, mockFile.Object, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("saved-test.jpg", result.ImagePath);
    }

    [Fact]
    public async Task GetImageAsync_ShouldReturnImageContent_WhenExists()
    {
        // Arrange
        var image = new Image { Id = 1, ImagePath = "test.jpg", EmployeeId = 1 };
        _dbContext.Images.Add(image);
        _dbContext.SaveChanges();

        File.WriteAllBytes("UploadedImages/test.jpg", new byte[] { 1, 2, 3, 4 });

        // Act
        var result = await _uploadImageService.GetImageAsync(1, CancellationToken.None);

        // Assert
        Assert.Equal("test.jpg", result.fileName);
        Assert.NotNull(result.fileContent);
        Assert.NotEmpty(result.fileContent);
    }

    [Fact]
    public async Task DeleteImageAsync_ShouldDeleteImageAndReturnTrue()
    {
        // Arrange
        var image = new Image { Id = 1, ImagePath = "test.jpg", EmployeeId = 1 };
        _dbContext.Images.Add(image);
        _dbContext.SaveChanges();

        _fileServiceMock.Setup(s => s.DeleteFileAsync("test.jpg")).Returns(Task.CompletedTask);

        // Act
        var result = await _uploadImageService.DeleteImageAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result);
    }
}
