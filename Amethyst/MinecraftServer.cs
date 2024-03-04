using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Hosting;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class MinecraftServer(
    MinecraftServerConfiguration configuration,
    IConnectionListenerFactory listenerFactory,
    ILoggerFactory loggerFactory) : IMinecraftServer
{
    private const int ProtocolVersion = 47;

    public ServerStatus Status => ServerStatus.Create(
        nameof(Amethyst),
        ProtocolVersion,
        configuration.MaximumPlayerCount,
        configuration.Description);

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

        var tasks = clients.Select(client => client.Value.StopAsync());
        logger.LogDebug("Stopping clients");
        await Task.WhenAll(tasks);
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

    private Task TickingAsync()
    {
        logger.LogInformation("Started ticking");
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        source.Dispose();

        if (listener is not null)
        {
            await listener.DisposeAsync();
        }

        var tasks = clients.Select(client => client.Value.DisposeAsync().AsTask());
        await Task.WhenAll(tasks);
    }
}