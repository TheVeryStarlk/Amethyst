using System.Net;
using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Events.Plugin;
using Amethyst.Api.Levels;
using Amethyst.Api.Levels.Generators;
using Amethyst.Extensions;
using Amethyst.Levels;
using Amethyst.Networking;
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
    private readonly Dictionary<int, Client> clients = [];

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (listener is not null)
        {
            throw new InvalidOperationException("Server has already started.");
        }

        var world = new World("Flat", new FlatWorldGenerator());

        Level = new Level
        {
            Worlds =
            {
                {
                    world.Name,
                    world
                }
            }
        };

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

    public async Task BroadcastPacketAsync(IOutgoingPacket packet)
    {
        await clients.Values
            .Where(client => client.Player is not null)
            .Select(client => client.Transport.Output.WritePacketAsync(packet))
            .WhenAll();
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
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(
                    "Unexpected exception from client: \"{Message}\"",
                    exception);
            }
            finally
            {
                clients.Remove(client.Identifier);
                logger.LogDebug("Removed client");
                await client.DisposeAsync();
            }
        }
    }

    private async Task TickingAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Started ticking");

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(cancellationToken);

                await clients.Values
                    .Where(client => client.State is ClientState.Playing)
                    .Select(client => client.KeepAliveAsync())
                    .WhenAll();
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