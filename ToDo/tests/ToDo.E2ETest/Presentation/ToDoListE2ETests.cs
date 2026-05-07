using System.Net.Http.Json;
using FluentAssertions;
using ToDo.API.DTO;
using ToDo.E2ETest.Shared;

namespace ToDo.E2ETest.Presentation;

public class ToDoListE2ETests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private HttpClient _httpClient = null!;

    public ValueTask InitializeAsync()
    {
        _httpClient = factory.CreateClient();
        _httpClient.BaseAddress = new Uri("http://localhost:5000");
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    [Fact]
    public async Task CreateToDos_Should_ReturnListOfToDos()
    {
        var beforeResult = await _httpClient.GetAsync("/todo", cancellationToken: TestContext.Current.CancellationToken);
        var beforeList = await beforeResult.Content.ReadFromJsonAsync<List<ToDoResponse>>(TestContext.Current.CancellationToken);
        var beforeCount = beforeList?.Count ?? 0;

        for (int i = 0; i < 10; i++)
        {
            var createRequest = new ToDoCreateRequest
            {
                Name = $"ToDo {i}",
                Description = $"Description {i}",
                Status = "New"
            };
            await _httpClient.PostAsJsonAsync("/todo", createRequest, cancellationToken: TestContext.Current.CancellationToken);
        }

        var result = await _httpClient.GetAsync("/todo", cancellationToken: TestContext.Current.CancellationToken);
        var resultList = await result.Content.ReadFromJsonAsync<List<ToDoResponse>>(TestContext.Current.CancellationToken);

        resultList.Should().NotBeNull();
        resultList.Count.Should().Be(beforeCount + 10);
    }
}
