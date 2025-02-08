using Amethyst.Components;
using Amethyst.Eventing;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Server(ILogger<Server> logger, EventDispatcher eventDispatcher) : IServer, IDisposable
{
    private CancellationTokenSource? source;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        return Task.CompletedTask;
    }

    public void Stop()
    {
        source!.Cancel();
    }

    public void Dispose()
    {
        source!.Dispose();
    }
}