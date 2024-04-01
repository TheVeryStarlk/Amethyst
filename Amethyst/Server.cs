using System.Collections.Concurrent;
using System.Net;
using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Events.Plugin;
using Amethyst.Api.Levels;
using Amethyst.Extensions;
using Amethyst.Networking;
using Amethyst.Networking.Packets.Playing;
using Amethyst.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Server(
    ServerConfiguration configuration,
    IConnectionListenerFactory listenerFactory,
    ILoggerFactory loggerFactory,
    PluginService pluginService,
    EventService eventService,
    CommandService commandService) : IServer
{
    public static int ProtocolVersion => 47;

    public ServerConfiguration Configuration => configuration;

    public IEnumerable<IPlayer> Players => clients.Values
        .Where(client => client.Player is not null)
        .Select(client => client.Player!);

    public ChatMessage Description { get; set; } = configuration.Description;

    public ILevel? Level { get; set; }

    public PluginService PluginService => pluginService;

    public CommandService CommandService => commandService;

    public EventService EventService => eventService;

    private IConnectionListener? listener;

    private readonly ILogger<Server> logger = loggerFactory.CreateLogger<Server>();
    private readonly ConcurrentDictionary<int, Client> clients = [];

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (listener is not null)
        {
            throw new InvalidOperationException("Server has already started.");
        }

        pluginService.Initialize();

        await eventService.ExecuteAsync(
            new PluginEnabledEventArgs
            {
                Server = this,
                DateTimeOffset = DateTimeOffset.Now
            });

        logger.LogInformation("Starting the server tasks");
        await Task.WhenAll(ListeningAsync(cancellationToken), TickingAsync(cancellationToken));

        await eventService.ExecuteAsync(
            new PluginDisabledEventArgs
            {
                Server = this,
                DateTimeOffset = DateTimeOffset.Now
            });
    }

    public async Task StopAsync()
    {
        await clients.Values
            .Select(client => client.StopAsync())
            .WhenAll();

        logger.LogInformation("Server stopped");
    }

    public async ValueTask DisposeAsync()
    {
        if (listener is not null)
        {
            await listener.DisposeAsync();
        }

        await clients.Values
            .Select(client => client.DisposeAsync().AsTask())
            .WhenAll();

        await pluginService.DisposeAsync();
    }

    public void QueuePacket(IPlayer player, IOutgoingPacket packet)
    {
        var client = clients.Values.FirstOrDefault(client => client.Player?.Username == player.Username);
        client?.Transport.Queue(packet);
    }

    public void BroadcastPacket(IOutgoingPacket packet, IWorld? world = null)
    {
        var players = clients.Values.Where(client => client.Player is not null);

        if (world is not null)
        {
            players = players.Where(client => client.Player!.World == world);
        }

        foreach (var client in players)
        {
            client.Transport.Queue(packet);
        }
    }

    public async Task BroadcastChatMessageAsync(ChatMessage message, ChatMessagePosition position = ChatMessagePosition.Box)
    {
        await Players
            .Select(player => player.SendChatMessageAsync(message, position))
            .WhenAll();

        logger.LogInformation("Broadcast: \"{Message}\"", message.Text);
    }

    public async Task DisconnectPlayerAsync(IPlayer player, ChatMessage reason)
    {
        await player.DisconnectAsync(reason);

        logger.LogInformation("Disconnected player: \"{Username}\", for: \"{Reason}\"",
            player.Username,
            reason.Text);
    }

    private async Task ListeningAsync(CancellationToken cancellationToken)
    {
        listener = await listenerFactory.BindAsync(new IPEndPoint(IPAddress.Any, configuration.ListeningPort), cancellationToken);
        logger.LogInformation("Started listening for connections at port {ListeningPort}", configuration.ListeningPort);

        var identifier = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var connection = await listener.AcceptAsync(cancellationToken);

                if (connection is null)
                {
                    logger.LogInformation("No longer accepting connections");
                    break;
                }

                logger.LogDebug("Accepted connection from: \"{EndPoint}\"", connection.RemoteEndPoint!.ToString());

                var client = new Client(
                    loggerFactory.CreateLogger<Client>(),
                    this,
                    connection,
                    identifier);

                clients[identifier] = client;
                identifier++;

                _ = ExecuteAsync(client);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    "Unexpected exception while listening for connections: \"{Message}\"",
                    exception);

                break;
            }
        }

        logger.LogCritical("Stopped listening for connections");

        // Ignore the parent method's cancellation token.
        await listener.UnbindAsync(CancellationToken.None);
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
                logger.LogDebug("Removed client");
                await client.DisposeAsync();
            }
        }
    }

    private async Task TickingAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Started ticking");

        var tick = 0;
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(5));

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(cancellationToken);

                var tasks = new List<Task>();

                for (var index = 0; index < clients.Values.Count; index++)
                {
                    var client = clients.Values.ElementAt(index);

                    if (client.State is not ClientState.Playing)
                    {
                        continue;
                    }

                    if (tick % 50 == 0)
                    {
                        if (client.MissedKeepAliveCount > Configuration.MaximumMissedKeepAliveCount)
                        {
                            await client.Player!.DisconnectAsync(ChatMessage.Create("Timed out.", Color.Red));
                            continue;
                        }

                        client.Transport.Queue(
                            new KeepAlivePacket
                            {
                                Payload = tick
                            });

                        client.MissedKeepAliveCount++;
                    }

                    tasks.Add(client.Transport.DequeueAsync(cancellationToken));
                }

                await Task.WhenAll(tasks);

                tick++;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    "Unexpected exception while ticking: \"{Message}\"",
                    exception);
            }
        }

        logger.LogCritical("Stopped ticking");

        var reason = ChatMessage.Create("Server stopped.", Color.Red);

        await Players
            .Select(player => DisconnectPlayerAsync(player, reason))
            .WhenAll();
    }
}