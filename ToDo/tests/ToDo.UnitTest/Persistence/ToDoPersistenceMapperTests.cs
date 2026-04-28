using FluentAssertions;
using ToDo.Domain.Models;
using ToDo.Persistence.Entities;
using ToDo.Persistence.Mappers;

namespace ToDo.UnitTest.Persistence;

public class ToDoPersistenceMapperTests
{
    [Fact]
    public void MapToEntity_MapFromModel_MapsCorrectly()
    {
        // Arrange
        var model = new ToDoModel
        {
            Name = "ToDo Model Name",
            Description = "ToDo Model Description",
            Status = "ToDo Model Status",
        };

        // Act
        var actual = model.MapToEntity();

        // Assert
        actual.Name.Should().Be(model.Name);
        actual.Description.Should().Be(model.Description);
        actual.Status.Should().Be(model.Status);
    }

    [Fact]
    public void MapToModel_MapFromEntityToModel_MapsCorrectly()
    {
        // Arrange
        var entity = new ToDoEntity
        {
            Id = 1,
            Name = "ToDo Entity Name",
            Description = "ToDo Entity Description",
            Status = "ToDo Entity Status",
            DateCreated = new DateTimeOffset(new DateTime(2026, 3, 10)),
            DateModified = new DateTimeOffset(new DateTime(2026, 6, 20)),
        };

        // Act
        var actual = entity.MapToModel();

        // Assert
        actual.Id.Should().Be(entity.Id);
        actual.Name.Should().Be(entity.Name);
        actual.Description.Should().Be(entity.Description);
        actual.Status.Should().Be(entity.Status);
        actual.DateCreated.Should().Be(entity.DateCreated);
        actual.DateModified.Should().Be(entity.DateModified);
    }
}
