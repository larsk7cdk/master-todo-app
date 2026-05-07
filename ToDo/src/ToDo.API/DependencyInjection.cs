using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using ToDo.API.Controllers;
using ToDo.API.Middlewares;

namespace ToDo.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        // Set Danish culture globally
        var cultureInfo = new CultureInfo("da-DK");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        // Global Exception Handler
        services.AddProblemDetails(configure =>
        {
            configure.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
            };
        });

        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddHttpContextAccessor();

        services
            .AddOpenApi().AddEndpointsApiExplorer();

        // Add Controllers with JSON support
        services.AddControllers(options => { options.Conventions.Add(new RouteTokenTransformerConvention(new LowerCaseParameterTransformer())); })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddFluentValidationAutoValidation(cfg => { cfg.DisableDataAnnotationsValidation = true; });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
