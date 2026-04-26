namespace ToDo.Application.Interfaces;

public interface ICrudRepository<TModel>
{
    Task<int> CreateAsync(TModel model, CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(TModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}