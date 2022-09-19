using Microsoft.AspNetCore.Routing;

namespace ReMediatR;

public interface IReMediatREndpointRouteBuilder
{
    IEndpointRouteBuilder Builder { get; }
    ReMediatROptions Options { get; }
}

internal class ReMediatREndpointRouteBuilder : IReMediatREndpointRouteBuilder
{
    public ReMediatREndpointRouteBuilder(IEndpointRouteBuilder builder, ReMediatROptions options)
    {
        Builder = builder;
        Options = options;
    }

    public IEndpointRouteBuilder Builder { get; }
    public ReMediatROptions Options { get; }
}