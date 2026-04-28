using ToDo.Domain.Models;

namespace ToDo.Application.Interfaces;

public interface ICrudRepository<TModel> where TModel : BaseModel
{
    Task<int> CreateAsync(TModel model, CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(TModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
