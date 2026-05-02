using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Application;
using ToDo.Application.Interfaces;
using ToDo.Application.Services;
using ToDo.Domain.Models;

namespace ToDo.UnitTest.Application;

public class DependencyInjectionTests
{
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddLogging();
        services.AddControllers();
        services.AddScoped<ICrudRepository<ToDoModel>, StubRepository>();
        services.AddApplication();

        return services.BuildServiceProvider();
    }

    [Fact]
    public void ToDoCreateRequestServiceKey_ResolvesToDoCreateRequestService()
    {
        // Arrange
        using var provider = BuildServiceProvider();

        // Act
        var service = provider.GetRequiredKeyedService<IRequestHandler<ToDoModel, int>>(KeyedServices.ToDoCreateRequestServiceKey);

        // Assert
        service.Should().BeOfType<ToDoCreateRequestService>();
    }

    [Fact]
    public void ToDoUpdateRequestServiceKey_ResolvesToDoUpdateRequestService()
    {
        // Arrange
        using var provider = BuildServiceProvider();

        // Act
        var service = provider.GetRequiredKeyedService<IRequestHandler<ToDoModel, int>>(KeyedServices.ToDoUpdateRequestServiceKey);

        // Assert
        service.Should().BeOfType<ToDoUpdateRequestService>();
    }

    [Fact]
    public void ToDoDeleteRequestServiceKey_ResolvesToDoDeleteRequestService()
    {
        // Arrange
        using var provider = BuildServiceProvider();

        // Act
        var service = provider.GetRequiredKeyedService<IRequestHandler<int>>(KeyedServices.ToDoDeleteRequestServiceKey);

        // Assert
        service.Should().BeOfType<ToDoDeleteRequestService>();
    }

    [Fact]
    public void ToDoReadAllRequestServiceKey_ResolvesToDoReadAllRequestService()
    {
        // Arrange
        using var provider = BuildServiceProvider();

        // Act
        var service = provider.GetRequiredKeyedService<IQueryHandler<IList<ToDoModel>>>(KeyedServices.ToDoReadAllRequestServiceKey);

        // Assert
        service.Should().BeOfType<ToDoReadAllRequestService>();
    }

    [Fact]
    public void ToDoReadDetailsRequestServiceKey_ResolvesToDoReadDetailsRequestService()
    {
        // Arrange
        using var provider = BuildServiceProvider();

        // Act
        var service = provider.GetRequiredKeyedService<IRequestHandler<int, ToDoModel>>(KeyedServices.ToDoReadDetailsRequestServiceKey);

        // Assert
        service.Should().BeOfType<ToDoReadDetailsRequestService>();
    }

    private sealed class StubRepository : ICrudRepository<ToDoModel>
    {
        public Task<int> CreateAsync(ToDoModel model, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<int> UpdateAsync(ToDoModel model, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task DeleteAsync(int id, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IReadOnlyList<ToDoModel>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ToDoModel>>([]);

        public Task<ToDoModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => Task.FromResult(new ToDoModel { Name = string.Empty, Description = string.Empty, Status = string.Empty });
    }
}
