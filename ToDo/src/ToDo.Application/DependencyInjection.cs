using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}
