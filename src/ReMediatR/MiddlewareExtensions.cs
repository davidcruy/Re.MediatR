using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace ReMediatR;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseReMediatR(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ReMediatRMiddleware>(Options.Create(new ReMediatROptions()));
    }

    public static IApplicationBuilder UseReMediatR(this IApplicationBuilder builder, string pattern, Action<IReMediatRBuilder> extraOptions)
    {
        var customBuilder = new ReMediatRBuilder(builder, new ReMediatROptions());
        extraOptions.Invoke(customBuilder);

        return builder.Map(pattern, applicationBuilder =>
        {
            applicationBuilder.UseMiddleware<ReMediatRMiddleware>(Options.Create(customBuilder.Options));
        });
    }
}