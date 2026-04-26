using Microsoft.EntityFrameworkCore;
using ToDo.Application.Interfaces;
using ToDo.Application.Models;
using ToDo.Persistence.DatabaseContext;
using ToDo.Persistence.Entities;
using ToDo.Shared.Application.Exceptions;

namespace ToDo.Persistence.Repositories;

public class ToDoRepository(AppDatabaseContext context) : ICrudRepository<ToDoModel>
{
    public async Task<int> CreateAsync(ToDoModel model, CancellationToken cancellationToken = default)
    {
        var entity = new ToDoEntity
        {
            Name = model.Name,
            Description = model.Description,
            Status = model.Status
        };

        await context.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<int> UpdateAsync(ToDoModel model, CancellationToken cancellationToken = default)
    {
        var entity = await GetEntityAsync(model.Id, asTracking: true, cancellationToken);

        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.Status = model.Status;

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetEntityAsync(id, asTracking: true, cancellationToken);

        context.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ToDoModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context
            .Set<ToDoEntity>()
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var models = entities.Select(entity => new ToDoModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Status = entity.Status,
            DateCreated = entity.DateCreated,
            DateModified = entity.DateModified
        }).ToList();

        return models;
    }

    public async Task<ToDoModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetEntityAsync(id, asTracking: false, cancellationToken);

        return new ToDoModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Status = entity.Status,
            DateCreated = entity.DateCreated,
            DateModified = entity.DateModified
        };
    }

    private async Task<ToDoEntity> GetEntityAsync(int id, bool asTracking = true, CancellationToken cancellationToken = default)
    {
        var query = context.Set<ToDoEntity>().AsQueryable();
        if (!asTracking) query = query.AsNoTracking();

        var entity = await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity ?? throw new NotFoundException(nameof(ToDoEntity), id);
    }
}
