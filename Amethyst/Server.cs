using System.Collections.Concurrent;
using Amethyst.Api;
using Amethyst.Api.Entities;
using Amethyst.Api.Plugins;
using Amethyst.Api.Plugins.Commands;
using Amethyst.Api.Plugins.Events;
using Amethyst.Api.Plugins.Events.Server;
using Amethyst.Api.Worlds;
using Amethyst.Components;
using Amethyst.Extensions;
using Amethyst.Plugins;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Server(
    ILoggerFactory loggerFactory,
    IConnectionListenerFactory listenerFactory,
    ServerOptions options,
    PluginService pluginService,
    EventService eventService,
    CommandService commandService) : IServer, IAsyncDisposable
{
    public int ProtocolVersion => 47;

    public ServerOptions Options => options;

    public IEnumerable<IPlayer> Players => clients.Values
        .Where(client => client.Player is not null)
        .Select(client => client.Player!);

    public IDictionary<string, IWorld> Worlds { get; set; } = new Dictionary<string, IWorld>();

    public IPluginService PluginService => pluginService;

    public IEventService EventService => eventService;

    public ICommandService CommandService => commandService;

    private CancellationTokenSource? source;
    private IConnectionListener? listener;

    private readonly ConcurrentDictionary<int, Client> clients = [];
    private readonly ILogger<IServer> logger = loggerFactory.CreateLogger<IServer>();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting the server tasks");
        source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        pluginService.Register();

        await eventService.ExecuteAsync(
            new ServerStartingEvent
            {
                Server = this,
                DateTimeOffset = DateTimeOffset.Now
            });

        await Task.WhenAll(ListeningAsync(), TickingAsync());
    }

    public async Task StopAsync()
    {
        if (source is null)
        {
            return;
        }

        logger.LogInformation("Stopping the server tasks");

        await eventService.ExecuteAsync(
            new ServerStoppingEvent
            {
                Server = this,
                DateTimeOffset = DateTimeOffset.Now
            });

        await source.CancelAsync();
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
            try
            {
                var connection = await listener.AcceptAsync(source.Token);

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

        var timer = new BalancingTimer(50, source.Token);

        while (await timer.WaitForNextTickAsync())
        {
            try
            {
                await clients.Values
                    .Select(client => client.TickAsync())
                    .WhenEach();
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

        foreach (var client in clients.Values)
        {
            client.Stop();
        }

        logger.LogInformation("Stopped ticking and closed all connections");
    }
}