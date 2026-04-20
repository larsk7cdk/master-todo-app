using Microsoft.EntityFrameworkCore;

namespace ToDo.Persistence.DatabaseContext;

/// <summary>
/// Base database context providing common audit functionality for all application contexts.
/// </summary>
/// <typeparam name="TContext">The derived context type.</typeparam>
public abstract class BaseAppDatabaseContext<TContext>(DbContextOptions<TContext> options) : DbContext(options)
    where TContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Loads all configurations that implement IEntityTypeConfiguration<T> from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}