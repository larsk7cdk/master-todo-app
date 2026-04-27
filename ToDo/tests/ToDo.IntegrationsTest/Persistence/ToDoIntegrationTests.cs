using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ToDo.Application.Models;
using ToDo.IntegrationsTest.Shared;
using ToDo.Persistence.DatabaseContext;
using ToDo.Persistence.Entities;
using ToDo.Persistence.Repositories;
using ToDo.Shared.Application.Exceptions;

namespace ToDo.IntegrationsTest.Persistence;

public class ToDoIntegrationTests : IAsyncLifetime
{
    private DatabaseFixture _fixture = null!;
    private AppDatabaseContext _appDatabaseContext = null!;
    private ToDoRepository _sut = null!;

    public async ValueTask InitializeAsync()
    {
        _fixture = new DatabaseFixture();
        await _fixture.StartAsync();

        var options = new DbContextOptionsBuilder<AppDatabaseContext>()
            .UseSqlServer(_fixture.ConnectionString)
            .Options;

        _appDatabaseContext = new AppDatabaseContext(options);
        _sut = new ToDoRepository(_appDatabaseContext);
    }

    public async ValueTask DisposeAsync()
    {
        await _fixture.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task CreateToDo_ShouldPersist_ReturnToDo()
    {
        // Arrange
        var todo = new ToDoModel
        {
            Name = "Test 1",
            Description = "Test description 1",
            Status = "New"
        };

        // Act
        var actual = await _sut.CreateAsync(todo, TestContext.Current.CancellationToken);

        // Assert
        var newToDo = await _appDatabaseContext.ToDos.FirstOrDefaultAsync(s => s.Name == todo.Name, TestContext.Current.CancellationToken);
        newToDo.Should().NotBeNull();
        newToDo.Id.Should().Be(actual);
        newToDo.Name.Should().Be(todo.Name);
        newToDo.Description.Should().Be(todo.Description);
        newToDo.Status.Should().Be(todo.Status);
        newToDo.DateCreated.Should().BeAfter(DateTimeOffset.UtcNow.AddSeconds(-30));
        newToDo.DateModified.Should().BeAfter(DateTimeOffset.UtcNow.AddSeconds(-30));
    }

    [Fact]
    public async Task CreateToDo_InvalidRequest_ReturnException()
    {
        // Arrange
        var todo = new ToDoModel
        {
            Name = "012345678901234567890123456789012345678901234567891",
            Description = "Test description 1",
            Status = "New"
        };

        // Act
        var actualEx = async () => await _sut.CreateAsync(todo, TestContext.Current.CancellationToken);

        // Assert
        await actualEx.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateToDo_ShouldPersist_ReturnUpdatedToDo()
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

        // Act
        var actualUpdate = await _sut.UpdateAsync(todoUpdate, TestContext.Current.CancellationToken);

        // Assert
        var updateToDo = await _appDatabaseContext.ToDos.FirstOrDefaultAsync(s => s.Id == actualUpdate, TestContext.Current.CancellationToken);
        updateToDo.Should().NotBeNull();
        updateToDo.Id.Should().Be(id);
        updateToDo.Name.Should().Be(todoUpdate.Name);
        updateToDo.Description.Should().Be(todoUpdate.Description);
        updateToDo.Status.Should().Be(todoUpdate.Status);
        updateToDo.DateCreated.Should().BeAfter(DateTimeOffset.UtcNow.AddSeconds(-30));
        updateToDo.DateModified.Should().BeAfter(DateTimeOffset.UtcNow.AddSeconds(-30));
    }

    [Fact]
    public async Task DeleteToDo_ShouldPersist_ReturnNoToDo()
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

        // Act
        await _sut.DeleteAsync(id, TestContext.Current.CancellationToken);
        var actualEx = async () => await _sut.GetByIdAsync(id, TestContext.Current.CancellationToken);

        // Assert
        await actualEx.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateMultipleToDo_ShouldPersist_ReturnToDos()
    {
        // Arrange
        List<ToDoEntity> entities = [];

        for (int i = 0; i < 10; i++)
        {
            entities.Add(new ToDoEntity
            {
                Name = $"Test {i}",
                Description = $"Test description {i}",
                Status = "New"
            });
        }

        _appDatabaseContext.ToDos.AddRange(entities);
        await _appDatabaseContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var actual = await _sut.GetAllAsync(TestContext.Current.CancellationToken);

        // Assert
        actual.Should().HaveCount(10);
        actual[0].Id.Should().BeGreaterThanOrEqualTo(1);
        actual[0].Name.Should().Be("Test 0");
        actual[0].Description.Should().Be("Test description 0");
        actual[0].Status.Should().Be("New");
        actual[0].DateCreated.Should().BeAfter(DateTimeOffset.UtcNow.AddSeconds(-30));
        actual[0].DateModified.Should().BeAfter(DateTimeOffset.UtcNow.AddSeconds(-30));
    }
}
