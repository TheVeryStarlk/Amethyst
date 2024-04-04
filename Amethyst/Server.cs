using System.Collections.Concurrent;
using Amethyst.Api;
using Amethyst.Api.Entities;
using Amethyst.Extensions;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Server(
    ILoggerFactory loggerFactory,
    IConnectionListenerFactory listenerFactory,
    ServerOptions options) : IServer, IAsyncDisposable
{
    public int ProtocolVersion => 47;

    public ServerOptions Options => options;

    public IEnumerable<IPlayer> Players => clients.Values
        .Where(client => client.Player is not null)
        .Select(client => client.Player!);

    private CancellationTokenSource? source;
    private IConnectionListener? listener;

    private readonly ConcurrentDictionary<int, Client> clients = [];
    private readonly ILogger<IServer> logger = loggerFactory.CreateLogger<IServer>();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting the server tasks");
        source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        return Task.WhenAll(ListeningAsync(), TickingAsync());
    }

    public void Stop()
    {
        logger.LogInformation("Stopping the server tasks");
        source?.Cancel();
    }

    public async ValueTask DisposeAsync()
    {
        source?.Dispose();

        if (listener is not null)
        {
            await listener.DisposeAsync();
        }

        await clients.Values
            .Select(client => client.DisposeAsync().AsTask())
            .WhenEach();
    }

    private async Task ListeningAsync()
    {
        ArgumentNullException.ThrowIfNull(source);

        listener = await listenerFactory.BindAsync(options.EndPoint, source.Token);
        logger.LogInformation("Started listening for connections at {EndPoint}", options.EndPoint.ToString());

        var identifier = 0;

        while (!source.IsCancellationRequested)
        {
            var connection = await listener.AcceptAsync();

            if (connection is null)
            {
                break;
            }

            var client = new Client(
                loggerFactory.CreateLogger<IClient>(),
                identifier,
                this,
                connection);

            clients[identifier++] = client;
            _ = ExecuteAsync(client);
        }

        await listener.UnbindAsync();
        logger.LogInformation("Stopped listening for connections");

        return;

        async Task ExecuteAsync(Client client)
        {
            await Task.Yield();

            try
            {
                await client.StartAsync();
            }
            finally
            {
                _ = clients.TryRemove(client.Identifier, out _);
                await client.DisposeAsync();
            }
        }
    }

    private async Task TickingAsync()
    {
        ArgumentNullException.ThrowIfNull(source);

        logger.LogInformation("Started ticking connections");

        while (!source.IsCancellationRequested)
        {
            await clients.Values
                .Where(client => client.Player is not null)
                .Select(client => client.TickAsync())
                .WhenEach();
        }

        foreach (var client in clients.Values)
        {
            client.Stop();
        }

        logger.LogInformation("Stopped ticking and closed all connections");
    }
}