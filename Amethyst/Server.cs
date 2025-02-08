using Amethyst.Components;
using Amethyst.Eventing;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Server(ILogger<Server> logger, EventDispatcher eventDispatcher) : IServer, IDisposable
{
    private readonly CancellationTokenSource source = new();

    public void Start()
    {

    }

    public void Stop()
    {
        source.Cancel();
    }

    public void Dispose()
    {
        source.Dispose();
    }
}