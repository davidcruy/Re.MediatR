using Microsoft.AspNetCore.Routing;

namespace ReMediatR;

public interface IReMediatREndpointRouteBuilder
{
    IEndpointRouteBuilder Builder { get; }
    ReMediatROptions Options { get; }
}

internal class ReMediatREndpointRouteBuilder(IEndpointRouteBuilder builder, ReMediatROptions options) : IReMediatREndpointRouteBuilder
{
    public IEndpointRouteBuilder Builder { get; } = builder;
    public ReMediatROptions Options { get; } = options;
}