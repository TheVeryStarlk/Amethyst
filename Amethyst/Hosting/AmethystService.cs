using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using Amethyst.Abstractions.Packets.Play;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amethyst.Hosting;

internal sealed class AmethystService(ILogger<AmethystService> logger, IOptions<AmethystOptions> options, ClientFactory clientFactory) : BackgroundService
{
    private readonly ConcurrentDictionary<Client, Task> clients = [];

    private CancellationTokenSource? source;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        using var listener = new Socket(options.Value.EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        listener.Bind(options.Value.EndPoint);
        listener.Listen();

        logger.LogInformation("Started listening on {EndPoint}", listener.LocalEndPoint);

        Debug.Assert(source is not null);

        while (true)
        {
            try
            {
                var socket = await listener.AcceptAsync(source.Token).ConfigureAwait(false);
                var client = clientFactory.Create(socket);

                clients[client] = InstantiateAsync(client);
                logger.LogDebug("Started client");
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

        var packet = new DisconnectPacket(options.Value.Message);
        
        foreach (var pair in clients)
        {
            pair.Key.Write(packet);
            pair.Key.Stop();
        }

        await Task.WhenAll(clients.Select(pair => pair.Value)).TimeoutAfter(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

        logger.LogInformation("Stopped listening");

        return;

        async Task InstantiateAsync(Client client)
        {
            await Task.Yield();

            await client.StartAsync().ConfigureAwait(false);

            client.Dispose();

            logger.LogDebug("Stopped client");

            if (!clients.TryRemove(client, out _))
            {
                logger.LogWarning("Failed to remove client");
            }
        }
    }

    public override void Dispose()
    {
        source?.Dispose();
    }
}