using System.Collections.Concurrent;
using Amethyst.Components;
using Amethyst.Components.Eventing.Sources.Server;
using Amethyst.Eventing;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Server(
    ILoggerFactory loggerFactory,
    IConnectionListenerFactory listenerFactory,
    EventDispatcher eventDispatcher) : IServer, IDisposable
{
    private readonly ILogger<Server> logger = loggerFactory.CreateLogger<Server>();
    private readonly ConcurrentDictionary<int, (Client Client, Task Task)> pairs = [];

    private CancellationTokenSource? source;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        return Task.WhenAll(ListeningAsync(), TickingAsync());
    }

    public void Stop()
    {
        source?.Cancel();
    }

    public void Dispose()
    {
        source?.Dispose();
    }

    private async Task ListeningAsync()
    {
        var starting = await eventDispatcher
            .DispatchAsync(this, new Starting(), source!.Token)
            .ConfigureAwait(false);

        await using var listener = await listenerFactory.BindAsync(starting.EndPoint, source.Token).ConfigureAwait(false);
        logger.LogInformation("Started listening");

        while (true)
        {
            try
            {
                var connection = await listener.AcceptAsync(source.Token).ConfigureAwait(false);

                var client = new Client(
                    loggerFactory.CreateLogger<Client>(),
                    connection!,
                    eventDispatcher,
                    Random.Shared.Next());

                pairs[client.Identifier] = (client, ExecuteAsync(client));
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected exception while listening");
                break;
            }
        }

        await listener.UnbindAsync().ConfigureAwait(false);
        logger.LogInformation("Stopped listening");

        var stopping = await eventDispatcher
            .DispatchAsync(this, new Stopping(), source.Token)
            .ConfigureAwait(false);

        foreach (var pair in pairs.Values)
        {
            pair.Client.Stop(stopping.Message);
        }

        logger.LogInformation("Waiting for clients to stop");
        await Task.WhenAll(pairs.Values.Select(pair => pair.Task)).TimeoutAfter(stopping.Timeout).ConfigureAwait(false);

        return;

        async Task ExecuteAsync(Client client)
        {
            await Task.Yield();

            await client.StartAsync().ConfigureAwait(false);
            await client.DisposeAsync().ConfigureAwait(false);

            if (!pairs.TryRemove(client.Identifier, out _))
            {
                logger.LogWarning("Failed to remove client");
            }
        }
    }

    private async Task TickingAsync()
    {
        while (true)
        {
            try
            {
                await Task.Delay(50, source!.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected exception while ticking");
                break;
            }
        }
    }
}