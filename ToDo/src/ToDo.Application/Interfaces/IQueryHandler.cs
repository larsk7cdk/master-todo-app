namespace ToDo.Application.Interfaces;

public interface IQueryHandler<TOut>
{
    Task<TOut> InvokeAsync(CancellationToken cancellationToken = default);
}
