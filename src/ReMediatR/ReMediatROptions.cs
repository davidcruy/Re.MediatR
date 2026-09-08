using System.Reflection;
using System.Text.Json;

namespace ReMediatR;

public class ReMediatROptions
{
    public Assembly RequestsAssembly { get; set; } = Assembly.GetExecutingAssembly();

    /// <summary>
    /// Build the type-cache with the fully qualified domain name of every request that will be registered.
    /// </summary>
    public bool IndexFullNameInTypeCache { get; set; }

    /// <summary>
    /// Serializer options used to deserialize the request body and serialize the response.
    /// Defaults to the ASP.NET Core "web" defaults (camelCase, case-insensitive property names, numbers readable from strings) with indented output.
    /// </summary>
    public JsonSerializerOptions SerializerOptions { get; set; } = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
}
