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
    private static JsonSerializerOptions _serializerOptions;

    public ReMediatRMiddleware(RequestDelegate next, IOptions<ReMediatROptions> options)
    {
        _next = next;
        _options = options.Value;
        _requestTypeCache = BuildTypeCache();
    }

    private Dictionary<string, Type> BuildTypeCache()
    {
        var types = _options.RequestsAssembly.GetTypes().Where(t => t.IsAssignableToGenericType(typeof(IRequest<>)));
        var typeCache = _options.IndexFullNameInTypeCache
            ? types.ToDictionary(t => t.FullName, t => t)
            : types.ToDictionary(t => t.Name, t => t);

        return typeCache;
    }

    public async Task InvokeAsync(HttpContext context, IMediator mediator)
    {
        if (context.Request.Method.Equals("POST"))
        {
            var type = context.Request.Query["type"];

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new Exception("Type query parameter was not set");
            }

            if (!_requestTypeCache.ContainsKey(type))
            {
                throw new Exception($"Type is not found in requests assembly: '{type}'");
            }

            var body = context.Request.Body;
            var requestType = _requestTypeCache[type];
            var options = EnsureOptions();

            var request = await JsonSerializer.DeserializeAsync(body, requestType, options, context.RequestAborted);
            if (request == null)
            {
                throw new Exception($"Request deserialization returned NULL for type '{type}'.");
            }

            var response = await mediator.Send(request);
            var responseJson = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(responseJson, context.RequestAborted);
        }
        else
        {
            await _next(context);
        }
    }

    private static JsonSerializerOptions EnsureOptions()
    {
        if (_serializerOptions != null)
            return _serializerOptions;
        
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        return _serializerOptions;
    }
}