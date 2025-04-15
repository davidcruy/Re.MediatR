using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace ReMediatR;

public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Map ReMediatr with the default URL '/mediatr' for an assembly of a specific type
    /// </summary>
    public static IEndpointConventionBuilder MapReMediatR(this IEndpointRouteBuilder endpoints, string pattern = "/mediatr", Action<IReMediatREndpointRouteBuilder> options = null)
    {
        var customBuilder = new ReMediatREndpointRouteBuilder(new ReMediatROptions());
        options?.Invoke(customBuilder);

        var app = endpoints.CreateApplicationBuilder();

        var pipeline = app
            .UseMiddleware<ReMediatRMiddleware>(Options.Create(customBuilder.Options))
            .Build();

        return endpoints.Map(pattern, pipeline);
    }
}