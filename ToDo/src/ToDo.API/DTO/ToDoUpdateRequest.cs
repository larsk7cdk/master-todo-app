namespace ToDo.API.DTO;

public class ToDoUpdateRequest
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string Status { get; init; }
}
