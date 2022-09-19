using System.Reflection;

namespace ReMediatR;

public class ReMediatROptions
{
    public ReMediatROptions()
    {
        RequestsAssembly = Assembly.GetExecutingAssembly();
    }

    public Assembly RequestsAssembly { get; set; }
}