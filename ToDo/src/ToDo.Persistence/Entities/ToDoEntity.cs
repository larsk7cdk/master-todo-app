using System.ComponentModel.DataAnnotations;

namespace ToDo.Persistence.Entities;

public class ToDoEntity : BaseEntity
{
    [MaxLength(50)]
    public required string Name { get; init; }

    [MaxLength(4000)]
    public required string Description { get; init; }

    [MaxLength(50)]
    public required string Status { get; init; }
}