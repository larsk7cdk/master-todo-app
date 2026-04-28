namespace ToDo.Domain.Models;

public class BaseModel
{
    public int Id { get; init; }

    public DateTimeOffset DateCreated { get; init; }

    public DateTimeOffset DateModified { get; init; }
}
