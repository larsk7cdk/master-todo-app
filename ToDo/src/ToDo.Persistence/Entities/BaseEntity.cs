using System.ComponentModel.DataAnnotations;

namespace ToDo.Persistence.Entities;

public class BaseEntity
{
    [Key] public int Id { get; init; }

    public DateTimeOffset DateCreated { get; set; }

    public DateTimeOffset DateModified { get; set; }
}