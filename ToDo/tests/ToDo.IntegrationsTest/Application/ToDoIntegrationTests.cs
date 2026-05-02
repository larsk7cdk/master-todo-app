using FluentAssertions;
using NSubstitute;
using ToDo.Application.Interfaces;
using ToDo.Application.Services;
using ToDo.Domain.Models;

namespace ToDo.IntegrationsTest.Application;

public class ToDoIntegrationTests : IAsyncLifetime
{
    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
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

        var repository = Substitute.For<ICrudRepository<ToDoModel>>();
        repository.CreateAsync(todo, Arg.Any<CancellationToken>()).Returns(1);

        var logger = Substitute.For<ILogger<ToDoCreateRequestService>>();
        logger.IsEnabled(LogLevel.Information).Returns(true);

        var sut = new ToDoCreateRequestService(repository, logger);

        // Act
        var actual = await sut.InvokeAsync(todo, TestContext.Current.CancellationToken);

        // Assert
        await repository.Received(1).CreateAsync(todo, Arg.Any<CancellationToken>());
        logger.ReceivedCalls().Should().ContainSingle(c => c.GetMethodInfo().Name == "Log");

        actual.Should().Be(1);
    }

    [Fact]
    public async Task UpdateInvokeAsync_WithValidModel_ShouldUpdateAndLog()
    {
        // Arrange
        var todo = new ToDoModel
        {
            Id = 1,
            Name = "Test 1",
            Description = "Test description 1",
            Status = "Ongoing"
        };

        var repository = Substitute.For<ICrudRepository<ToDoModel>>();
        repository.UpdateAsync(todo, Arg.Any<CancellationToken>()).Returns(todo.Id);

        var logger = Substitute.For<ILogger<ToDoUpdateRequestService>>();
        logger.IsEnabled(LogLevel.Information).Returns(true);

        var sut = new ToDoUpdateRequestService(repository, logger);

        // Act
        var actual = await sut.InvokeAsync(todo, TestContext.Current.CancellationToken);

        // Assert
        await repository.Received(1).UpdateAsync(todo, Arg.Any<CancellationToken>());
        logger.ReceivedCalls().Should().ContainSingle(c => c.GetMethodInfo().Name == "Log");

        actual.Should().Be(1);
    }

    [Fact]
    public async Task DeleteInvokeAsync_WithValidId_ShouldDeleteAndLog()
    {
        // Arrange
        const int id = 1;

        var repository = Substitute.For<ICrudRepository<ToDoModel>>();

        var logger = Substitute.For<ILogger<ToDoDeleteRequestService>>();
        logger.IsEnabled(LogLevel.Information).Returns(true);

        var sut = new ToDoDeleteRequestService(repository, logger);

        // Act
        await sut.InvokeAsync(id, TestContext.Current.CancellationToken);

        // Assert
        await repository.Received(1).DeleteAsync(id, Arg.Any<CancellationToken>());
        logger.ReceivedCalls().Should().ContainSingle(c => c.GetMethodInfo().Name == "Log");
    }

    [Fact]
    public async Task ReadAllInvokeAsync_ShouldReadAndLog()
    {
        // Arrange
        var todos = Enumerable.Range(0, 10).Select(i => new ToDoModel
        {
            Id = i,
            Name = "Test 1",
            Description = "Test description 1",
            Status = "Ongoing"
        }).ToList();

        var repository = Substitute.For<ICrudRepository<ToDoModel>>();
        repository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(todos);

        var logger = Substitute.For<ILogger<ToDoReadAllRequestService>>();
        logger.IsEnabled(LogLevel.Information).Returns(true);

        var sut = new ToDoReadAllRequestService(repository, logger);

        // Act
        var actual = await sut.InvokeAsync(TestContext.Current.CancellationToken);

        // Assert
        await repository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        logger.ReceivedCalls().Should().ContainSingle(c => c.GetMethodInfo().Name == "Log");

        actual.Should().HaveCount(10);
    }

    [Fact]
    public async Task ReadDetailsInvokeAsync_WithValidId_ShouldReadAndLog()
    {
        // Arrange
        const int id = 1;

        var repository = Substitute.For<ICrudRepository<ToDoModel>>();
        repository.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(new ToDoModel
            {
                Id = id,
                Name = "Test 1",
                Description = "Test description 1",
                Status = "Ongoing"
            });

        var logger = Substitute.For<ILogger<ToDoReadDetailsRequestService>>();
        logger.IsEnabled(LogLevel.Information).Returns(true);

        var sut = new ToDoReadDetailsRequestService(repository, logger);

        // Act
        var actual = await sut.InvokeAsync(id, TestContext.Current.CancellationToken);

        // Assert
        await repository.Received(1).GetByIdAsync(id, Arg.Any<CancellationToken>());
        logger.ReceivedCalls().Should().ContainSingle(c => c.GetMethodInfo().Name == "Log");

        actual.Id.Should().Be(id);
        actual.Name.Should().Be("Test 1");
        actual.Description.Should().Be("Test description 1");
        actual.Status.Should().Be("Ongoing");
    }
}
