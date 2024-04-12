using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace ReMediatR;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapReMediatR<T>(this IEndpointRouteBuilder endpoints, string pattern)
    {
        return endpoints.MapReMediatR(pattern, o =>
        {
            o.Options.RequestsAssembly = typeof(T).Assembly;
        });
    }

    public static IEndpointConventionBuilder MapReMediatR(this IEndpointRouteBuilder endpoints, string pattern, Action<IReMediatREndpointRouteBuilder> optionsDelegate = default)
    {
        var customBuilder = new ReMediatREndpointRouteBuilder(endpoints, new ReMediatROptions());
        optionsDelegate?.Invoke(customBuilder);

        var app = endpoints.CreateApplicationBuilder();

        var pipeline = app
            .UseMiddleware<ReMediatRMiddleware>(Options.Create(customBuilder.Options))
            .Build();

        return endpoints.Map(pattern, pipeline);
    }
}