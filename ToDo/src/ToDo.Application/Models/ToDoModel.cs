namespace ToDo.Application.Models;

public class ToDoModel : BaseModel
{
    // Navn på To-Do emnet
    public required string Name { get; init; }

    // Beskrivelse af To-Do emnet
    public required string Description { get; init; }

    // Status
    public required string Status { get; init; }
}