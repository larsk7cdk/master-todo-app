namespace ToDo.Application.Models;

public class BaseModel
{
    public int Id { get; init; }

    public DateTime DateCreated { get; init; }

    public DateTime DateModified { get; init; }
}