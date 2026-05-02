using Microsoft.Extensions.Logging;
using ToDo.Application.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.Application.Services;

public partial class ToDoUpdateRequestService(
    ICrudRepository<ToDoModel> repository,
    ILogger<ToDoUpdateRequestService> logger) : IRequestHandler<ToDoModel, int>
{
    public async Task<int> InvokeAsync(ToDoModel model, CancellationToken cancellationToken = default)
    {
        LogToDoUpdate(model.Id);

        var id = await repository.UpdateAsync(model, cancellationToken);
        return id;
    }

    [LoggerMessage(LogLevel.Information, "Updating ToDo with ID: {id}")]
    partial void LogToDoUpdate(int id);
}
