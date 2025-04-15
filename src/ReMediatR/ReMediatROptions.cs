using System.Reflection;

namespace ReMediatR;

public class ReMediatROptions
{
    public Assembly RequestsAssembly { get; set; } = Assembly.GetExecutingAssembly();

    /// <summary>
    /// Build the type-cache with the fully qualified domain name of every request that will be registered
    /// </summary>
    public bool IndexFullNameInTypeCache { get; set; }
}