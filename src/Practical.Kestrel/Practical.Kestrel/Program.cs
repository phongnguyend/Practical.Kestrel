using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

var serverOptions = new KestrelServerOptions();
serverOptions.ListenAnyIP(5059);

var server = new KestrelServer(
    Options.Create(serverOptions),
    new SocketTransportFactory(Options.Create(new SocketTransportOptions()), new NullLoggerFactory()),
    new NullLoggerFactory());

await server.StartAsync(new MyHttpApplication(), CancellationToken.None);

Console.ReadLine();

class MyHttpApplicationContext
{
    public required HttpContext HttpContext { get; set; }
}

class MyHttpApplication : IHttpApplication<MyHttpApplicationContext>
{
    public MyHttpApplicationContext CreateContext(IFeatureCollection contextFeatures)
    {
        return new MyHttpApplicationContext
        {
            HttpContext = new DefaultHttpContext(contextFeatures)
        };
    }

    public void DisposeContext(MyHttpApplicationContext context, Exception? exception)
    {
    }

    public async Task ProcessRequestAsync(MyHttpApplicationContext context)
    {
        var rq = context.HttpContext.Request;
        var rs = context.HttpContext.Response;

        if (rq.Path.Equals("/"))
        {
            rs.StatusCode = 200;
            await using var writer = new StreamWriter(rs.Body);
            await writer.WriteAsync("Hello World!");
        }
        else
        {
            rs.StatusCode = 400;
        }
    }
}