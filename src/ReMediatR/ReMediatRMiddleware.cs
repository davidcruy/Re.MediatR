using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ReMediatr;

namespace ReMediatR;

public class ReMediatRMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDictionary<string, Type> _requestTypeCache;
    private readonly ReMediatROptions _options;

    public ReMediatRMiddleware(RequestDelegate next, IOptions<ReMediatROptions> options)
    {
        _next = next;
        _options = options.Value;
        _requestTypeCache = BuildTypeCache();
    }

    private IDictionary<string, Type> BuildTypeCache()
    {
        var types = _options.RequestsAssembly.GetTypes().Where(t => t.IsAssignableToGenericType(typeof(IRequest<>)));
        var typeCache = types.ToDictionary(t => t.FullName, t => t);

        return typeCache;
    }

    public async Task InvokeAsync(HttpContext context, IMediator mediator)
    {
        if (context.Request.Method.Equals("POST"))
        {
            var type = context.Request.Query["type"];
            if (!string.IsNullOrWhiteSpace(type))
            {
                if (!_requestTypeCache.ContainsKey(type))
                {
                    throw new Exception($"Type is not found in requests assembly: '{type}'");
                }

                var req = context.Request.Body;

                var requestType = _requestTypeCache[type];
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                dynamic? request = await JsonSerializer.DeserializeAsync(req, requestType, options, context.RequestAborted);
                if (request == null)
                {
                    throw new Exception($"Request deserialization returned NULL for type '{type}'.");
                }

                var response = await mediator.Send(request);
                var responseJson = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                });

                await HttpResponseWritingExtensions.WriteAsync(context.Response, responseJson, context.RequestAborted);
                return;
            }
        }

        await _next(context);
    }
}