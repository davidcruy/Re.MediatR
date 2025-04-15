using Microsoft.AspNetCore.Builder;

namespace ReMediatR;

public interface IReMediatRBuilder
{
    IApplicationBuilder Builder { get; }
    ReMediatROptions Options { get; }
}

internal class ReMediatRBuilder(IApplicationBuilder builder, ReMediatROptions options) : IReMediatRBuilder
{
    public IApplicationBuilder Builder { get; } = builder;
    public ReMediatROptions Options { get; } = options;
}