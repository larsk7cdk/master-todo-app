using System.ComponentModel.DataAnnotations;

namespace ToDo.Persistence.Entities;

public class ToDoEntity : BaseEntity
{
    // Navn på To-Do emnet
    [MaxLength(50)] 
    public required string Name { get; init; }

    // Valgfri beskrivelse af To-Do emnet
    [MaxLength(4000)]
    public required string Description { get; init; }

    // Status enum
    [MaxLength(50)]
    public required string Status { get; init; }
}