using Microsoft.Extensions.DependencyInjection;
using ToDo.Application.Interfaces;
using ToDo.Application.Services;
using ToDo.Domain.Models;

namespace ToDo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Add RequestHandlers
        services
            .AddKeyedScoped<IRequestHandler<ToDoModel, int>, ToDoCreateRequestService>(KeyedServices.ToDoCreateRequestServiceKey)
            .AddKeyedScoped<IRequestHandler<ToDoModel, int>, ToDoUpdateRequestService>(KeyedServices.ToDoUpdateRequestServiceKey)
            .AddKeyedScoped<IRequestHandler<int>, ToDoDeleteRequestService>(KeyedServices.ToDoDeleteRequestServiceKey)
            .AddKeyedScoped<IQueryHandler<IList<ToDoModel>>, ToDoReadAllRequestService>(KeyedServices.ToDoReadAllRequestServiceKey)
            .AddKeyedScoped<IRequestHandler<int, ToDoModel>, ToDoReadDetailsRequestService>(KeyedServices.ToDoReadDetailsRequestServiceKey);

        return services;
    }
}
