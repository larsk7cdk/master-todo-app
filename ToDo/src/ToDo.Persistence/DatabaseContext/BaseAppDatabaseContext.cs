using Microsoft.EntityFrameworkCore;
using ToDo.Persistence.Entities;

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

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in base.ChangeTracker.Entries<BaseEntity>()
                     .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified))
        {
            entry.Entity.DateModified = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.DateCreated = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}