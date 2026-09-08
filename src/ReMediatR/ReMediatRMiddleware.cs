using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ReMediatr;

namespace ReMediatR;

public class ReMediatRMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Dictionary<string, Type> _requestTypeCache;
    private readonly ReMediatROptions _options;

    public ReMediatRMiddleware(RequestDelegate next, IOptions<ReMediatROptions> options)
    {
        _next = next;
        _options = options.Value;
        _requestTypeCache = BuildTypeCache();
    }

    /// <summary>
    /// Both IRequest&lt;TResponse&gt; and the non-generic IRequest (MediatR 12: a separate interface, not IRequest&lt;Unit&gt;) are exposed;
    /// the latter answers with an empty JSON object.
    /// </summary>
    private Dictionary<string, Type> BuildTypeCache()
    {
        var types = _options.RequestsAssembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.IsAssignableToGenericType(typeof(IRequest<>)) || typeof(IRequest).IsAssignableFrom(t));

        var typeCache = _options.IndexFullNameInTypeCache
            ? types.ToDictionary(t => t.FullName, t => t)
            : types.ToDictionary(t => t.Name, t => t);

        return typeCache;
    }

    public async Task InvokeAsync(HttpContext context, IMediator mediator)
    {
        if (!HttpMethods.IsPost(context.Request.Method))
        {
            await _next(context);
            return;
        }

        var cancellationToken = context.RequestAborted;
        string type = context.Request.Query["type"];

        if (string.IsNullOrWhiteSpace(type))
        {
            await WriteError(context, StatusCodes.Status400BadRequest, "Type query parameter was not set", cancellationToken);
            return;
        }

        if (!_requestTypeCache.TryGetValue(type, out var requestType))
        {
            await WriteError(context, StatusCodes.Status400BadRequest, $"Type is not found in requests assembly: '{type}'", cancellationToken);
            return;
        }

        var options = _options.SerializerOptions;
        var request = await JsonSerializer.DeserializeAsync(context.Request.Body, requestType, options, cancellationToken);
        if (request == null)
        {
            await WriteError(context, StatusCodes.Status400BadRequest, $"Request deserialization returned NULL for type '{type}'.", cancellationToken);
            return;
        }

        var response = await mediator.Send(request, cancellationToken);
        var responseJson = JsonSerializer.Serialize(response, options);

        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsync(responseJson, cancellationToken);
    }

    private static Task WriteError(HttpContext context, int statusCode, string message, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "text/plain; charset=utf-8";
        return context.Response.WriteAsync(message, cancellationToken);
    }
}
