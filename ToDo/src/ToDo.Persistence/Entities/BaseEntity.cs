using System.ComponentModel.DataAnnotations;

namespace ToDo.Persistence.Entities;

public class BaseEntity
{
    [Key] public int Id { get; init; }

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }
}