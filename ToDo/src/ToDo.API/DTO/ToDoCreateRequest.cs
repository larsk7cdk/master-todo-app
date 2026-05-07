namespace ToDo.API.DTO;

public class ToDoCreateRequest
{
    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string Status { get; init; }
}
