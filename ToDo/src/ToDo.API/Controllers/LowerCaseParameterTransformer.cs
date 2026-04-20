using Microsoft.AspNetCore.Routing;

namespace ToDo.API.Controllers;

public class LowerCaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        return value?.ToString()?.ToLowerInvariant();
    }
}