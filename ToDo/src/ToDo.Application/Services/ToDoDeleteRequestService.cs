using Microsoft.Extensions.Logging;
using ToDo.Application.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.Application.Services;

public partial class ToDoDeleteRequestService(
    ICrudRepository<ToDoModel> repository,
    ILogger<ToDoDeleteRequestService> logger) : IRequestHandler<int>
{
    public async Task InvokeAsync(int id, CancellationToken cancellationToken = default)
    {
        LogToDoDelete(id);

        await repository.DeleteAsync(id, cancellationToken);
    }

    [LoggerMessage(LogLevel.Information, "Updating ToDo with ID: {id}")]
    partial void LogToDoDelete(int id);
}
