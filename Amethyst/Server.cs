using System.Collections.Concurrent;
using Amethyst.Eventing;
using Amethyst.Hosting;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Server(
    ILoggerFactory loggerFactory,
    IConnectionListenerFactory listenerFactory,
    EventDispatcher eventDispatcher,
    AmethystOptions options) : IDisposable
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
        await using var listener = await listenerFactory.BindAsync(options.EndPoint, source!.Token);

        logger.LogInformation("Started listening");

        while (true)
        {
            try
            {
                var connection = await listener.AcceptAsync(source.Token);
                var client = new Client(loggerFactory.CreateLogger<Client>(), connection!, Random.Shared.Next());

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

        await listener.UnbindAsync();

        logger.LogInformation("Stopped listening");

        foreach (var pair in pairs.Values)
        {
            pair.Client.Stop("No reason provided.");
        }

        logger.LogInformation("Waiting for clients to disconnect");

        await Task
            .WhenAll(pairs.Values.Select(pair => pair.Task))
            .TimeoutAfter(TimeSpan.FromSeconds(5));

        return;

        async Task ExecuteAsync(Client client)
        {
            await Task.Yield();

            await client.StartAsync();
            await client.DisposeAsync();

            if (!pairs.TryRemove(client.Identifier, out _))
            {
                logger.LogWarning("Failed to remove client");
            }
        }
    }

    private Task TickingAsync()
    {
        return Task.CompletedTask;
    }
}