Re.MediatR
==========

Exposes [MediatR](https://github.com/jbogard/MediatR) trough a Web-API endpoint for SPA applications *(Angular, React, Vue, ...)*

You can now use MediatR without having to write your own API Controllers

Works with **net6.0** only at the moment

Just register the middleware and you can start calling MediatR from inside your favorite SPA framework

### Installing Re.MediatR

You should install [Re.MediatR with NuGet](https://www.nuget.org/packages/Re.MediatR):

    Install-Package Re.MediatR

Or via the .NET Core command line interface:

    dotnet add package Re.MediatR

### Usage

Just add ReMediatR as a new endpoint during runtime configuration:

```C#
app.UseEndpoints(endpoints =>
{
    endpoints.MapReMediatR<PingRequest>("/mediatr");
});
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
    public PingResponse()
    {
        Time = DateTime.Now;
    }

    public DateTime Time { get; }
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
