using System.Diagnostics;
using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Hosting;
using Amethyst.Plugins;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class MinecraftServer(
    MinecraftServerConfiguration configuration,
    IConnectionListenerFactory listenerFactory,
    ILoggerFactory loggerFactory,
    PluginService pluginService) : IMinecraftServer
{
    public const int ProtocolVersion = 47;

    public ServerStatus Status { get; } = ServerStatus.Create(
        nameof(Amethyst),
        ProtocolVersion,
        configuration.MaximumPlayerCount,
        configuration.Description);

    public IEnumerable<IPlayer> Players => clients.Values
        .Where(client => client.Player is not null)
        .Select(client => client.Player!);

    public PluginService PluginService => pluginService;

    private IConnectionListener? listener;

    private readonly ILogger<MinecraftServer> logger = loggerFactory.CreateLogger<MinecraftServer>();
    private readonly CancellationTokenSource source = new CancellationTokenSource();
    private readonly Dictionary<int, MinecraftClient> clients = [];

    public Task StartAsync()
    {
        if (listener is not null)
        {
            throw new InvalidOperationException("Server has already started.");
        }

        pluginService.Load();

        logger.LogInformation("Starting the server tasks");
        return Task.WhenAll(ListeningAsync(), TickingAsync());
    }

    public async Task StopAsync()
    {
        logger.LogInformation("Stopping the server");

        await source.CancelAsync();

        if (listener is not null)
        {
            await listener.UnbindAsync();
        }

        var reason = ChatMessage.Create("Server stopped.", Color.Red);

        logger.LogDebug("Stopping clients");

        var tasks = clients.Values.Select(
            client => client.Player is not null
                ? client.Player.DisconnectAsync(reason)
                : client.StopAsync());

        await Task.WhenAll(tasks);
    }

    public async ValueTask DisposeAsync()
    {
        source.Dispose();

        if (listener is not null)
        {
            await listener.DisposeAsync();
        }

        await pluginService.DisposeAsync();

        var tasks = clients.Values.Select(client => client.DisposeAsync().AsTask());
        await Task.WhenAll(tasks);
    }

    public async Task BroadcastChatMessageAsync(ChatMessage message, ChatMessagePosition position = ChatMessagePosition.Box)
    {
        logger.LogInformation("Broadcasting: \"{Message}\"", message.Text);

        foreach (var player in Players)
        {
            await player.SendChatMessageAsync(message, position);
        }
    }

    public async Task DisconnectPlayerAsync(IPlayer player, ChatMessage reason)
    {
        logger.LogInformation("Disconnected player: \"{Username}\", for: \"{Reason}\"",
            player.Username,
            reason.Text);

        await player.DisconnectAsync(reason);
    }

    private async Task ListeningAsync()
    {
        listener = await listenerFactory.BindAsync(configuration.ListeningEndPoint, source.Token);
        logger.LogInformation("Started listening for connections at: \"{EndPoint}\"", configuration.ListeningEndPoint);

        var identifier = 0;

        while (!source.IsCancellationRequested)
        {
            try
            {
                var connection = await listener.AcceptAsync(source.Token);

                if (connection is null)
                {
                    logger.LogInformation("No longer accepting connections");
                    break;
                }

                logger.LogDebug("Accepted connection from: \"{EndPoint}\"", connection.RemoteEndPoint!.ToString());

                var client = new MinecraftClient(
                    loggerFactory.CreateLogger<MinecraftClient>(),
                    this,
                    connection,
                    identifier);

                clients[identifier] = client;
                identifier++;

                _ = ExecuteAsync(client);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(
                    "Unexpected exception while listening for connections: \"{Message}\"",
                    exception.Message);
            }
        }

        return;

        async Task ExecuteAsync(MinecraftClient client)
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
                    exception.Message);
            }
            finally
            {
                logger.LogDebug("Removing client");
                clients.Remove(client.Identifier);

                await client.StopAsync();
                await client.DisposeAsync();
            }
        }
    }

    private async Task TickingAsync()
    {
        logger.LogInformation("Started ticking");

        var timer = new BalancingTimer(50, source.Token);

        while (await timer.WaitForNextTickAsync())
        {
            try
            {
                // Soon.
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(
                    "Unexpected exception while ticking: \"{Message}\"",
                    exception.Message);
            }
        }
    }
}

internal sealed class BalancingTimer(int milliseconds, CancellationToken cancellationToken)
{
    private long delay;

    private readonly Stopwatch stopwatch = new Stopwatch();
    private readonly long ticksInterval = milliseconds * Stopwatch.Frequency / 1000L;

    public async ValueTask<bool> WaitForNextTickAsync()
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        var delta = stopwatch.ElapsedTicks;
        stopwatch.Restart();

        delay += delta - ticksInterval;

        if (delay >= 0)
        {
            return true;
        }

        var extraMilliseconds = (int) (-delay * 1000L / Stopwatch.Frequency);
        await Task.Delay(extraMilliseconds, cancellationToken);
        return true;
    }
}