Re.MediatR
==========

Exposes [MediatR](https://github.com/jbogard/MediatR) trough a Web-API endpoint for SPA applications *(Angular, React, Vue, ...)*

You can now use MediatR without having to write your own API Controllers

Targets **net8.0**, **net9.0** and **net10.0** (MediatR 12)

Just register the middleware and you can start calling MediatR from inside your favorite SPA framework

### Installing Re.MediatR

You should install [Re.MediatR with NuGet](https://www.nuget.org/packages/Re.MediatR):

    Install-Package Re.MediatR

Or via the .NET Core command line interface:

    dotnet add package Re.MediatR

### Usage

Just add ReMediatR as a new endpoint during runtime configuration:

```C#
// Map ReMediatr with the default URL '/mediatr', register all requests in the current executing assembly
app.MapReMediatR();
```

Where **PingRequest** is a generic type for which ReMediatR will scan the assembly of that type

Given this MediatR request-handler:

```C#

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyApp.RequestHandlers;

public class PingRequest : IRequest<PingResponse>
{
    public string Hello { get; set; }
}

public class PingResponse
{
    public DateTime Time { get; } = DateTime.Now;
}

public class PingRequestHandler : IRequestHandler<PingRequest, PingResponse>
{
    public override Task<PingResponse> Handle(PingRequest request, CancellationToken token)
    {
        return Task.FromResult(new PingResponse());
    }
}
```

The handler is now available trough a HTTP-post request, if your dotnet app is running on localhost:5001 write

```http

POST http://localhost:5001/mediatr?type=MyApp.RequestHandlers.PingRequest
Content-Type: application/json

{
  "hello": "world"
}
```

Response

```http

HTTP/1.1 200 OK
Content-Type: application/json
Content-Length: xy

{
  "time": "2022-02-13T15:31:55.559Z"
}
```

### Options

When setting up ReMediatR, you can change the name of the endpoint 'mediatr' to something else, and you can change additional settings.

```C#

app.MapReMediatR("/mediatr", o => o
    .RequestAssembly(typeof(PingRequest).Assembly) // Provide another assembly that contains the requests
    .IndexFullNameInTypeCache() // Index all requests with their fully qualified type name, including the namespace
    .ConfigureJson(json => json.Converters.Add(new MyConverter())) // Tweak the System.Text.Json options
);
```

### Behaviour

- Both `IRequest<TResponse>` and the non-generic `IRequest` are exposed; the latter answers `{}`.
- JSON uses the ASP.NET Core *web* defaults (camelCase, case-insensitive, numbers may be quoted) with indented output; adjust via `ConfigureJson`.
- A missing or unknown `type` answers `400 Bad Request`. Exceptions thrown by handlers are not translated — map them in your own middleware (e.g. `UnauthorizedAccessException` → 401/403).
- Authorization is the host's job: chain `.RequireAuthorization()` on the returned endpoint builder and/or enforce per-request rules in a MediatR pipeline behavior.