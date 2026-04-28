namespace ToDo.Domain.Models;

public class ToDoModel : BaseModel
{
    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string Status { get; init; }
}
