using System.Collections.Concurrent;
using System.Net.Sockets;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Networking.Packets;
using Amethyst.Abstractions.Networking.Packets.Login;
using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Eventing.Player;
using Amethyst.Eventing.Server;
using Amethyst.Worlds;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Server(ILoggerFactory loggerFactory, EventDispatcher eventDispatcher) : IServer, IDisposable
{
    private readonly ILogger<Server> logger = loggerFactory.CreateLogger<Server>();
    private readonly ConcurrentDictionary<Client, Task> clients = [];

    private CancellationTokenSource? source;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        return Task.WhenAll(ListeningAsync(), TickingAsync());
    }

    public void Dispose()
    {
        source?.Dispose();
    }

    private async Task ListeningAsync()
    {
        var starting = eventDispatcher.Dispatch(this, new Starting());

        using var listener = new Socket(starting.EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        listener.Bind(starting.EndPoint);
        listener.Listen();

        logger.LogInformation("Started listening");

        while (true)
        {
            try
            {
                var socket = await listener.AcceptAsync(source!.Token).ConfigureAwait(false);
                var client = new Client(loggerFactory.CreateLogger<Client>(), eventDispatcher, socket);

                clients[client] = ExecuteAsync(client);
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

        var stopping = eventDispatcher.Dispatch(this, new Stopping());

        foreach (var pair in clients)
        {
            IOutgoingPacket packet = pair.Key.State switch
            {
                State.Handshake or State.Status or State.Login => new FailurePacket(stopping.Message),
                State.Play => new DisconnectPacket(stopping.Message),
                _ => throw new ArgumentOutOfRangeException()
            };

            pair.Key.Write(packet);
            pair.Key.Stop();
        }

        await Task.WhenAll(clients.Select(pair => pair.Value)).TimeoutAfter(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

        logger.LogInformation("Stopped listening");

        return;

        async Task ExecuteAsync(Client client)
        {
            await Task.Yield();

            await client.StartAsync().ConfigureAwait(false);
            client.Dispose();

            if (client.Player is { } player)
            {
                eventDispatcher.Dispatch(player, new Left());
            }

            logger.LogDebug("Stopped client");

            if (!clients.TryRemove(client, out _))
            {
                logger.LogWarning("Failed to remove client");
            }
        }
    }

    private async Task TickingAsync()
    {
        var tick = new Tick();
        var timer = new BalancingTimer(TimeSpan.FromSeconds(0.05));

        while (true)
        {
            try
            {
                if (!await timer.WaitForNextTickAsync(source!.Token).ConfigureAwait(false))
                {
                    break;
                }

                eventDispatcher.Dispatch(this, tick);
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