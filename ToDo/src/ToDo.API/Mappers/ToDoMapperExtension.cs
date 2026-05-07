using ToDo.API.DTO;
using ToDo.Domain.Models;

namespace ToDo.API.Mappers;

public static class ToDoMapperExtension
{
    public static ToDoModel ToModel(this ToDoCreateRequest request) => new()
    {
        Name = request.Name,
        Description = request.Description,
        Status = request.Status
    };

    public static ToDoModel ToModel(this ToDoUpdateRequest request) => new()
    {
        Id = request.Id,
        Name = request.Name,
        Description = request.Description,
        Status = request.Status
    };

    public static ToDoResponse ToResponse(this ToDoModel model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        Description = model.Description,
        Status = model.Status,
        DateCreated = model.DateCreated,
        DateModified = model.DateModified
    };
}
