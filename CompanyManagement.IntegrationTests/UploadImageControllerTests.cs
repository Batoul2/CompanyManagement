using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.IO;

public class UploadImageControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UploadImageControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UploadImage_ShouldReturnOk_WhenValidImageUploaded()
    {
        // Arrange
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("dummy image content"));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        content.Add(fileContent, "ImageFile", "test.jpg");

        // Act
        var response = await _client.PostAsync("/api/UploadImage/1/upload", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseBody);
    }

    [Fact]
    public async Task GetImage_ShouldReturnImage_WhenImageExists()
    {
        // Act
        var response = await _client.GetAsync("/api/UploadImage/3");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/jpeg", response.Content.Headers.ContentType.ToString());
    }

    [Fact]
    public async Task DeleteImage_ShouldReturnOk_WhenImageDeleted()
    {
        // Act
        var response = await _client.DeleteAsync("/api/UploadImage/delete/2");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
