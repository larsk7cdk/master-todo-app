using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ToDo.API.DTO;
using ToDo.E2ETest.Shared;

namespace ToDo.E2ETest.Presentation;

public class ToDoE2ETests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
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
    public async Task CreateToDo_Should_ReturnSuccessStatusCode()
    {
        var createRequest = new ToDoCreateRequest
        {
            Name = "First ToDo",
            Description = "First Description",
            Status = "New"
        };
        var createResponse = await _httpClient.PostAsJsonAsync("/todo", createRequest, cancellationToken: TestContext.Current.CancellationToken);

        createResponse.Should().NotBeNull();
        createResponse.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task CreateToDo_Should_ReturnStatusCodeBadRequest()
    {
        var createRequest = new ToDoCreateRequest
        {
            Name = "",
            Description = "First Description",
            Status = "New"
        };
        var createResponse = await _httpClient.PostAsJsonAsync("/todo", createRequest, cancellationToken: TestContext.Current.CancellationToken);

        createResponse.Should().NotBeNull();
        createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await createResponse.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        problem.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateToDoAndRead_Should_ReturnCreatedToDo()
    {
        var createRequest = new ToDoCreateRequest
        {
            Name = "First ToDo",
            Description = "First Description",
            Status = "New"
        };
        var createResponse = await _httpClient.PostAsJsonAsync("/todo", createRequest, cancellationToken: TestContext.Current.CancellationToken);

        var result = await createResponse.Content.ReadFromJsonAsync<JsonNode>(TestContext.Current.CancellationToken);
        var id = result!["id"]!.GetValue<int>();

        var readResponse = await _httpClient.GetFromJsonAsync<ToDoResponse>($"/todo/{id}", cancellationToken: TestContext.Current.CancellationToken);

        readResponse.Should().NotBeNull();
        readResponse.Id.Should().Be(id);
        readResponse.Name.Should().Be(createRequest.Name);
        readResponse.Description.Should().Be(createRequest.Description);
        readResponse.Status.Should().Be(createRequest.Status);
        readResponse.DateCreated.Should().BeAfter(DateTimeOffset.UtcNow.AddSeconds(-30));
        readResponse.DateModified.Should().BeAfter(DateTimeOffset.UtcNow.AddSeconds(-30));
    }

    [Fact]
    public async Task CreateToDoAndUpdate_Should_ReturnUpdatedToDo()
    {
        var createRequest = new ToDoCreateRequest
        {
            Name = "First ToDo",
            Description = "First Description",
            Status = "New"
        };
        var createResponse = await _httpClient.PostAsJsonAsync("/todo", createRequest, cancellationToken: TestContext.Current.CancellationToken);

        var result = await createResponse.Content.ReadFromJsonAsync<JsonNode>(TestContext.Current.CancellationToken);
        var id = result!["id"]!.GetValue<int>();

        var readCreateResponse =
            await _httpClient.GetFromJsonAsync<ToDoResponse>($"/todo/{id}", cancellationToken: TestContext.Current.CancellationToken);

        readCreateResponse.Should().NotBeNull();
        readCreateResponse.Id.Should().Be(id);
        readCreateResponse.Name.Should().Be(createRequest.Name);
        readCreateResponse.Description.Should().Be(createRequest.Description);
        readCreateResponse.Status.Should().Be(createRequest.Status);
        readCreateResponse.DateCreated.Should().BeAfter(DateTimeOffset.UtcNow.AddSeconds(-30));
        readCreateResponse.DateModified.Should().BeAfter(DateTimeOffset.UtcNow.AddSeconds(-30));

        var updateRequest = new ToDoUpdateRequest
        {
            Id = id,
            Name = "First ToDo",
            Description = "First Description",
            Status = "New"
        };
        var updateResponse = await _httpClient.PutAsJsonAsync("/todo", updateRequest, cancellationToken: TestContext.Current.CancellationToken);

        var updateResult = await updateResponse.Content.ReadFromJsonAsync<JsonNode>(TestContext.Current.CancellationToken);
        var updateId = updateResult!["id"]!.GetValue<int>();

        var readUpdateResponse =
            await _httpClient.GetFromJsonAsync<ToDoResponse>($"/todo/{updateId}", cancellationToken: TestContext.Current.CancellationToken);

        readUpdateResponse.Should().NotBeNull();
        readUpdateResponse.Id.Should().Be(id);
        readUpdateResponse.Name.Should().Be(createRequest.Name);
        readUpdateResponse.Description.Should().Be(createRequest.Description);
        readUpdateResponse.Status.Should().Be(createRequest.Status);
        readUpdateResponse.DateCreated.Should().BeAfter(DateTimeOffset.UtcNow.AddSeconds(-30));
        readUpdateResponse.DateModified.Should().BeAfter(DateTimeOffset.UtcNow.AddSeconds(-30));
    }

    [Fact]
    public async Task CreateToDoAndDelete_Should_ReturnCreatedStatusCodeOk()
    {
        var createRequest = new ToDoCreateRequest
        {
            Name = "First ToDo",
            Description = "First Description",
            Status = "New"
        };
        var createResponse = await _httpClient.PostAsJsonAsync("/todo", createRequest, cancellationToken: TestContext.Current.CancellationToken);

        var result = await createResponse.Content.ReadFromJsonAsync<JsonNode>(TestContext.Current.CancellationToken);
        var id = result!["id"]!.GetValue<int>();

        var deleteResponse = await _httpClient.DeleteAsync($"/todo/{id}", cancellationToken: TestContext.Current.CancellationToken);

        deleteResponse.Should().NotBeNull();
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
