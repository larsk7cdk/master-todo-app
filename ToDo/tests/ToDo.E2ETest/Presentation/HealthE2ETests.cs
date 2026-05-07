using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ToDo.E2ETest.Presentation;

public class HealthE2ETests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _httpClient = factory.CreateClient();

    [Fact]
    public async Task Health_Should_ReturnOk()
    {
        // Arrange
        _httpClient.BaseAddress = new Uri("http://localhost:5000");

        // Act
        var response = await _httpClient.GetAsync("/health", cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        factory.Dispose();
        GC.SuppressFinalize(this);
    }
}
