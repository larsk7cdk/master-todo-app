using Microsoft.Extensions.Logging;
using ToDo.Application.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.Application.Services;

public partial class ToDoReadAllRequestService(
    ICrudRepository<ToDoModel> repository,
    ILogger<ToDoReadAllRequestService> logger) : IQueryHandler<IList<ToDoModel>>
{
    public async Task<IList<ToDoModel>> InvokeAsync(CancellationToken cancellationToken = default)
    {
        LogToDoReadAll();

        var model = await repository.GetAllAsync(cancellationToken);
        return model.ToList();
    }

    [LoggerMessage(LogLevel.Information, "Read All ToDo's")]
    partial void LogToDoReadAll();
}
