using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ToDo.IntegrationsTest.Shared;
using ToDo.Persistence.DatabaseContext;
using ToDo.Persistence.Entities;

namespace ToDo.IntegrationsTest.Persistence;

public class ToDoIntegrationTests : IAsyncLifetime
{
    private AppDatabaseContext _sut = null!;
    private DatabaseFixture _fixture = null!;

    public async ValueTask InitializeAsync()
    {
        _fixture = new DatabaseFixture();
        await _fixture.StartAsync();

        var options = new DbContextOptionsBuilder<AppDatabaseContext>()
            .UseSqlServer(_fixture.ConnectionString)
            .Options;

        _sut = new AppDatabaseContext(options);
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
        var todo = new ToDoEntity
        {
            Name = "Test 1",
            Description = "Test description 1",
            Status = "New"
        };
        _sut.ToDos.Add(todo);

        // Act
        await _sut.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        var result = _sut.ToDos.FirstOrDefault(s => s.Name == todo.Name);
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThanOrEqualTo(1);
        result.Name.Should().Be(todo.Name);
        result.Description.Should().Be(todo.Description);
        result.Status.Should().Be(todo.Status);
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
        _sut.ToDos.Add(todo);

        // Act
        await _sut.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        var result = _sut.ToDos.FirstOrDefault(s => s.Name == todo.Name);
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThanOrEqualTo(1);
        result.Name.Should().Be(todo.Name);
        result.Description.Should().Be(todo.Description);
        result.Status.Should().Be(todo.Status);


        // Arrange
        var todoUpdate = new ToDoEntity
        {
            Name = "Test 2",
            Description = "Test description 2",
            Status = "Ongoing"
        };
        _sut.ToDos.Add(todoUpdate);

        // Act
        await _sut.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        var resultUpdated = _sut.ToDos.FirstOrDefault(s => s.Name == todoUpdate.Name);
        resultUpdated.Should().NotBeNull();
        resultUpdated.Id.Should().BeGreaterThanOrEqualTo(result.Id);
        resultUpdated.Name.Should().Be(todoUpdate.Name);
        resultUpdated.Description.Should().Be(todoUpdate.Description);
        resultUpdated.Status.Should().Be(todoUpdate.Status);
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
        _sut.ToDos.Add(todo);

        // Act
        await _sut.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        var result = _sut.ToDos.FirstOrDefault(s => s.Name == todo.Name);
        result.Should().NotBeNull();

        // Arrange
        _sut.ToDos.Remove(todo);

        // Act
        await _sut.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        var resultDeleted = _sut.ToDos.FirstOrDefault(s => s.Name == todo.Name);
        resultDeleted.Should().BeNull();
    }

    [Fact]
    public async Task CreateMultipleToDo_ShouldPersist_ReturnToDos()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            var todo = new ToDoEntity
            {
                Name = $"Test {i}",
                Description = $"Test description {i}",
                Status = "New"
            };
            _sut.ToDos.Add(todo);
        }

        // Act
        await _sut.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Assert
        var result = _sut.ToDos;
        result.Should().NotBeNull();
        result.Count().Should().Be(10);
    }
}