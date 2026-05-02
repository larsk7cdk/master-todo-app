using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Application.Interfaces;
using ToDo.Application.Services;
using ToDo.Domain.Models;

namespace ToDo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Add validation
        services
            .AddFluentValidationAutoValidation(cfg => { cfg.DisableDataAnnotationsValidation = true; })
            .AddValidatorsFromAssembly(assembly);


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
