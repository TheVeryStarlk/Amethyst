using System.Collections.Concurrent;
using Amethyst.Api;
using Amethyst.Api.Entities;
using Amethyst.Extensions;
using Microsoft.AspNetCore.Connections;

namespace Amethyst;

internal sealed class Server : IServer
{
    public int ProtocolVersion => 47;

    public ServerOptions Options { get; }

    public IEnumerable<IPlayer> Players { get; }

    private CancellationTokenSource? source;

    private readonly ConcurrentDictionary<int, Client> clients = [];

    public Task StartAsync(CancellationToken cancellationToken)
    {
        source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        return Task.WhenAll(ListeningAsync(), TickingAsync());
    }

    public void Stop()
    {
        source?.Cancel();
    }

    private async Task ListeningAsync()
    {
        ArgumentNullException.ThrowIfNull(source);

        var identifier = 0;

        while (!source.IsCancellationRequested)
        {
            var client = new Client(identifier, this, new DefaultConnectionContext());
            clients[identifier++] = client;
            await ExecuteAsync(client);
        }

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

        while (!source.IsCancellationRequested)
        {
            await clients.Values
                .Where(client => client.Player is not null)
                .Select(client => client.TickAsync())
                .WhenEach();
        }
    }
}