using System.Reflection;

namespace ReMediatR;

public interface IReMediatREndpointRouteBuilder
{
    /// <summary>
    /// Set the assembly that contains the requests that will be exposed through the mediatr endpoint
    /// </summary>
    IReMediatREndpointRouteBuilder RequestAssembly(Assembly assembly);

    /// <summary>
    /// Index all requests with their fully qualified type name, including the namespace
    /// </summary>
    IReMediatREndpointRouteBuilder IndexFullNameInTypeCache();
}

internal class ReMediatREndpointRouteBuilder(ReMediatROptions options) : IReMediatREndpointRouteBuilder
{
    internal ReMediatROptions Options { get; } = options;

    public IReMediatREndpointRouteBuilder RequestAssembly(Assembly assembly)
    {
        Options.RequestsAssembly = assembly;
        return this;
    }

    public IReMediatREndpointRouteBuilder IndexFullNameInTypeCache()
    {
        Options.IndexFullNameInTypeCache = true;
        return this;
    }
}