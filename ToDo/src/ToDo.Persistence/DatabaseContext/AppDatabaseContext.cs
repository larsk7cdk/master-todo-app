using Microsoft.EntityFrameworkCore;

namespace ToDo.Persistence.DatabaseContext;

/// <summary>
/// Database context for Factory service.
/// Inherits common audit functionality from BaseAppDatabaseContext.
/// </summary>
public sealed class AppDatabaseContext(DbContextOptions<AppDatabaseContext> options)
    : BaseAppDatabaseContext<AppDatabaseContext>(options)
{
    // DbSets for entities
}