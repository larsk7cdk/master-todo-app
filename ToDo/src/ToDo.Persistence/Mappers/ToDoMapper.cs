using ToDo.Application.Models;
using ToDo.Persistence.Entities;

namespace ToDo.Persistence.Mappers;

public static class ToDoMapper
{
    public static ToDoEntity MapToEntity(this ToDoModel model) => new()
    {
        Name = model.Name,
        Description = model.Description,
        Status = model.Status
    };

    public static ToDoModel MapToModel(this ToDoEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Status = entity.Status,
        DateCreated = entity.DateCreated,
        DateModified = entity.DateModified
    };
}
