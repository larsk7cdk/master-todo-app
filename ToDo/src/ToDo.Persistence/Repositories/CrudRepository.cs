using Microsoft.EntityFrameworkCore;
using ToDo.Application.Interfaces;
using ToDo.Application.Models;
using ToDo.Persistence.DatabaseContext;
using ToDo.Persistence.Entities;
using ToDo.Shared.Application.Exceptions;

namespace ToDo.Persistence.Repositories;

public class ToDoRepository<TModel>(AppDatabaseContext context) : ICrudRepository<TModel>
    where TModel : ToDoModel
{
    public async Task<int> CreateAsync(TModel model, CancellationToken cancellationToken = default)
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

    public async Task<int> UpdateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        var entity = await GetEntityAsync(model.Id, cancellationToken);

        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.Status = model.Status;

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetEntityAsync(id, cancellationToken);

        context.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context
            .Set<ToDoEntity>()
            .AsNoTracking()
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

        return (IReadOnlyList<TModel>)models;
    }

    public async Task<TModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetEntityAsync(id, cancellationToken);

        return entity != null
            ? (TModel?)new ToDoModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Status = entity.Status,
                DateCreated = entity.DateCreated,
                DateModified = entity.DateModified
            }
            : throw new NotFoundException(typeof(TModel).Name, id);
    }

    private async Task<ToDoEntity> GetEntityAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await context
            .Set<ToDoEntity>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity ?? throw new NotFoundException(nameof(ToDoEntity), id);
    }
}