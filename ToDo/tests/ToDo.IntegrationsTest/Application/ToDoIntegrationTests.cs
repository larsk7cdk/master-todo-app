using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using ToDo.Application.Services;
using ToDo.Domain.Models;
using ToDo.IntegrationsTest.Shared;
using ToDo.Persistence.DatabaseContext;
using ToDo.Persistence.Entities;
using ToDo.Persistence.Repositories;
using ToDo.Shared.Application.Exceptions;

namespace ToDo.IntegrationsTest.Application;

public class ToDoIntegrationTests : IAsyncLifetime
{
    private DatabaseFixture _fixture = null!;
    private AppDatabaseContext _appDatabaseContext = null!;
    private ToDoRepository _repository = null!;

    public async ValueTask InitializeAsync()
    {
        _fixture = new DatabaseFixture();
        await _fixture.StartAsync();

        var options = new DbContextOptionsBuilder<AppDatabaseContext>()
            .UseSqlServer(_fixture.ConnectionString)
            .Options;

        _appDatabaseContext = new AppDatabaseContext(options);
        _repository = new ToDoRepository(_appDatabaseContext);
    }

    public async ValueTask DisposeAsync()
    {
        await _fixture.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task CreateInvokeAsync_WithValidModel_ShouldCreateAndLog()
    {
        // Arrange
        var todo = new ToDoModel
        {
            Name = "Test 1",
            Description = "Test description 1",
            Status = "New"
        };

        var logger = Substitute.For<ILogger<ToDoCreateRequestService>>();
        logger.IsEnabled(LogLevel.Information).Returns(true);

        var sut = new ToDoCreateRequestService(_repository, logger);

        // Act
        var actual = await sut.InvokeAsync(todo, TestContext.Current.CancellationToken);
        var actualToDo = await _appDatabaseContext.ToDos
            .FirstOrDefaultAsync(x => x.Id == actual, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        actualToDo.Should().NotBeNull();
        actualToDo.Name.Should().Be(todo.Name);
        actualToDo.Description.Should().Be(todo.Description);
        actualToDo.Status.Should().Be(todo.Status);

        actual.Should().Be(actualToDo.Id);

        logger.ReceivedCalls().Should().ContainSingle(c => c.GetMethodInfo().Name == "Log");
    }

    [Fact]
    public async Task UpdateInvokeAsync_WithValidModel_ShouldUpdateAndLog()
    {
        // Arrange
        var todo = new ToDoEntity
        {
            Name = "Test 1",
            Description = "Test description 1",
            Status = "New"
        };

        _appDatabaseContext.ToDos.Add(todo);
        await _appDatabaseContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        var id = todo.Id;

        var todoUpdate = new ToDoModel
        {
            Id = id,
            Name = "Test 2",
            Description = "Test description 2",
            Status = "Ongoing"
        };

        var logger = Substitute.For<ILogger<ToDoUpdateRequestService>>();
        logger.IsEnabled(LogLevel.Information).Returns(true);

        var sut = new ToDoUpdateRequestService(_repository, logger);

        // Act
        var actual = await sut.InvokeAsync(todoUpdate, TestContext.Current.CancellationToken);
        var actualToDo = await _appDatabaseContext.ToDos
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        actualToDo.Should().NotBeNull();
        actual.Should().Be(actualToDo.Id);
        actualToDo.Name.Should().Be(todoUpdate.Name);
        actualToDo.Description.Should().Be(todoUpdate.Description);
        actualToDo.Status.Should().Be(todoUpdate.Status);

        logger.ReceivedCalls().Should().ContainSingle(c => c.GetMethodInfo().Name == "Log");
    }

    [Fact]
    public async Task DeleteInvokeAsync_WithValidId_ShouldDeleteAndLog()
    {
        // Arrange
        var todo = new ToDoEntity
        {
            Name = "Test 1",
            Description = "Test description 1",
            Status = "New"
        };

        _appDatabaseContext.ToDos.Add(todo);
        await _appDatabaseContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        var id = todo.Id;

        var logger = Substitute.For<ILogger<ToDoDeleteRequestService>>();
        logger.IsEnabled(LogLevel.Information).Returns(true);

        var sut = new ToDoDeleteRequestService(_repository, logger);

        // Act
        await sut.InvokeAsync(id, TestContext.Current.CancellationToken);

        var actual = await _appDatabaseContext.ToDos
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        actual.Should().BeNull();

        logger.ReceivedCalls().Should().ContainSingle(c => c.GetMethodInfo().Name == "Log");
    }

    [Fact]
    public async Task ReadAllInvokeAsync_ShouldReadAndLog()
    {
        // Arrange
        var todos = Enumerable.Range(0, 10).Select(i => new ToDoEntity
        {
            Name = $"Test {i}",
            Description = $"Test description {i}",
            Status = "New"
        }).ToList();

        await _appDatabaseContext.AddRangeAsync(todos, TestContext.Current.CancellationToken);
        await _appDatabaseContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var logger = Substitute.For<ILogger<ToDoReadAllRequestService>>();
        logger.IsEnabled(LogLevel.Information).Returns(true);

        var sut = new ToDoReadAllRequestService(_repository, logger);

        // Act
        var actual = await sut.InvokeAsync(TestContext.Current.CancellationToken);

        // Assert
        actual.Count.Should().Be(10);

        logger.ReceivedCalls().Should().ContainSingle(c => c.GetMethodInfo().Name == "Log");
    }

    [Fact]
    public async Task ReadDetailsInvokeAsync_WithValidId_ShouldReadAndLog()
    {
        // Arrange
        var todo = new ToDoEntity
        {
            Name = "Test 1",
            Description = "Test description 1",
            Status = "New"
        };

        _appDatabaseContext.ToDos.Add(todo);
        await _appDatabaseContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var logger = Substitute.For<ILogger<ToDoReadDetailsRequestService>>();
        logger.IsEnabled(LogLevel.Information).Returns(true);

        var sut = new ToDoReadDetailsRequestService(_repository, logger);

        // Act
        var actual = await sut.InvokeAsync(todo.Id, TestContext.Current.CancellationToken);

        // Assert
        actual.Should().NotBeNull();
        actual.Id.Should().Be(todo.Id);
        actual.Name.Should().Be(todo.Name);
        actual.Description.Should().Be(todo.Description);
        actual.Status.Should().Be(todo.Status);

        logger.ReceivedCalls().Should().ContainSingle(c => c.GetMethodInfo().Name == "Log");
    }

    [Fact]
    public async Task ReadDetailsInvokeAsync_WithInValidId_ShouldThrowExceptionAndLog()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ToDoReadDetailsRequestService>>();
        logger.IsEnabled(LogLevel.Information).Returns(true);

        var sut = new ToDoReadDetailsRequestService(_repository, logger);

        // Act
        var actualEx = async () => await sut.InvokeAsync(9999, TestContext.Current.CancellationToken);

        // Assert
        await actualEx.Should().ThrowAsync<NotFoundException>();

        logger.ReceivedCalls().Should().ContainSingle(c => c.GetMethodInfo().Name == "Log");
    }
}
