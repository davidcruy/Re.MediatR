using System.Reflection;
using System.Text.Json;

namespace ReMediatR;

public interface IReMediatREndpointRouteBuilder
{
    /// <summary>
    /// Set the assembly that contains the requests that will be exposed through the mediatr endpoint.
    /// </summary>
    IReMediatREndpointRouteBuilder RequestAssembly(Assembly assembly);

    /// <summary>
    /// Index all requests with their fully qualified type name, including the namespace.
    /// </summary>
    IReMediatREndpointRouteBuilder IndexFullNameInTypeCache();

    /// <summary>
    /// Adjust the <see cref="JsonSerializerOptions"/> used for the request body and the response (e.g. add converters).
    /// Starts from the ASP.NET Core "web" defaults.
    /// </summary>
    IReMediatREndpointRouteBuilder ConfigureJson(Action<JsonSerializerOptions> configure);
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

    public IReMediatREndpointRouteBuilder ConfigureJson(Action<JsonSerializerOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(Options.SerializerOptions);
        return this;
    }
}
