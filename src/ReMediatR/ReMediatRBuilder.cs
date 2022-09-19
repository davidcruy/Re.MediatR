using Microsoft.AspNetCore.Builder;

namespace ReMediatR;

public interface IReMediatRBuilder
{
    IApplicationBuilder Builder { get; }
    ReMediatROptions Options { get; }
}

internal class ReMediatRBuilder : IReMediatRBuilder
{
    public ReMediatRBuilder(IApplicationBuilder builder, ReMediatROptions options)
    {
        Builder = builder;
        Options = options;
    }

    public IApplicationBuilder Builder { get; }
    public ReMediatROptions Options { get; }
}