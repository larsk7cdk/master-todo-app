using Microsoft.EntityFrameworkCore;
using ToDo.Application.Interfaces;
using ToDo.Application.Models;
using ToDo.Persistence.DatabaseContext;
using ToDo.Persistence.Entities;
using ToDo.Persistence.Mappers;
using ToDo.Shared.Application.Exceptions;

namespace ToDo.Persistence.Repositories;

public class ToDoRepository(AppDatabaseContext context) : ICrudRepository<ToDoModel>
{
    public async Task<int> CreateAsync(ToDoModel model, CancellationToken cancellationToken = default)
    {
        var entity = model.MapToEntity();

        await context.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task<int> UpdateAsync(ToDoModel model, CancellationToken cancellationToken = default)
    {
        var entity = await GetEntityAsync(model.Id, asTracking: true, cancellationToken);

        context.Entry(entity).CurrentValues.SetValues(new
        {
            model.Name,
            model.Description,
            model.Status
        });

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
        var models = await context
            .Set<ToDoEntity>()
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(entity => entity.MapToModel())
            .ToListAsync(cancellationToken);

        return models;
    }

    public async Task<ToDoModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetEntityAsync(id, asTracking: false, cancellationToken);
        return entity.MapToModel();
    }

    private async Task<ToDoEntity> GetEntityAsync(int id, bool asTracking, CancellationToken cancellationToken = default)
    {
        var query = context.Set<ToDoEntity>().AsQueryable();
        if (!asTracking) query = query.AsNoTracking();

        var entity = await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity ?? throw new NotFoundException(nameof(ToDoEntity), id);
    }
}
