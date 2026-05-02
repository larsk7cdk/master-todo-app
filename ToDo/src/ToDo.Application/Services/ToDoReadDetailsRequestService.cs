using Microsoft.Extensions.Logging;
using ToDo.Application.Interfaces;
using ToDo.Domain.Models;
using ToDo.Shared.Application.Exceptions;

namespace ToDo.Application.Services;

public partial class ToDoReadDetailsRequestService(
    ICrudRepository<ToDoModel> repository,
    ILogger<ToDoReadDetailsRequestService> logger) : IRequestHandler<int, ToDoModel>
{
    public async Task<ToDoModel> InvokeAsync(int id, CancellationToken cancellationToken = default)
    {
        LogToDoReadDetails(id);

        var model = await repository.GetByIdAsync(id, cancellationToken);
        return model ?? throw new NotFoundException(nameof(ToDoModel), id);
    }

    [LoggerMessage(LogLevel.Information, "Read details for ToDo with ID: {id}")]
    partial void LogToDoReadDetails(int id);
}
