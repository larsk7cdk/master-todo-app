using Microsoft.EntityFrameworkCore;
using ToDo.Persistence.Entities;

namespace ToDo.Persistence.DatabaseContext;

/// <summary>
/// Database context.
/// Inherits common audit functionality from BaseAppDatabaseContext.
/// </summary>
public sealed class AppDatabaseContext(DbContextOptions<AppDatabaseContext> options)
    : BaseAppDatabaseContext<AppDatabaseContext>(options)
{
    // DbSets for entities
    public DbSet<ToDoEntity> ToDos { get; set; }
}