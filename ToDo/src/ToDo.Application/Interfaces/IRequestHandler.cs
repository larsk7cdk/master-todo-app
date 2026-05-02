namespace ToDo.Application.Interfaces;

public interface IRequestHandler<in T>
{
    Task InvokeAsync(T value, CancellationToken cancellationToken = default);
}

public interface IRequestHandler<in TIn, TOut>
{
    Task<TOut> InvokeAsync(TIn value, CancellationToken cancellationToken = default);
}

public interface IResponseHandler<TOut>
{
    Task<TOut> InvokeAsync(CancellationToken cancellationToken = default);
}
