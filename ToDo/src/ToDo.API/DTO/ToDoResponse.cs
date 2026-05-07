namespace ToDo.API.DTO;

public class ToDoResponse
{
    public int? Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required string Status { get; init; }

    public required DateTimeOffset DateCreated { get; init; }

    public required DateTimeOffset DateModified { get; init; }
}
