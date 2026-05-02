using Microsoft.Extensions.Logging;
using ToDo.Application.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.Application.Services;

public partial class ToDoCreateRequestService(
    ICrudRepository<ToDoModel> repository,
    ILogger<ToDoCreateRequestService> logger) : IRequestHandler<ToDoModel, int>
{
    public async Task<int> InvokeAsync(ToDoModel model, CancellationToken cancellationToken = default)
    {
        LogToDoCreate(model.Name);

        var id = await repository.CreateAsync(model, cancellationToken);
        return id;
    }

    [LoggerMessage(LogLevel.Information, "Creating new ToDo with Name: {name}")]
    partial void LogToDoCreate(string name);
}
