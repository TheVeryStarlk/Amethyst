using System.Net;
using Amethyst.Api;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amethyst;

internal sealed class MinecraftServer(IPEndPoint listeningEndPoint, ILoggerFactory loggerFactory) : IMinecraftServer
{
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

        if (listener is not null)
        {
            await listener.UnbindAsync();
        }

        await source.CancelAsync();

        var tasks = new Task[clients.Count];

        for (var index = 0; index < clients.Count; index++)
        {
            tasks[index] = clients[index].StopAsync();
        }

        logger.LogDebug("Stopping clients");
        await Task.WhenAll(tasks);
    }

    private async Task ListeningAsync()
    {
        var factory = new SocketTransportFactory(
            Options.Create(new SocketTransportOptions()),
            loggerFactory);

        listener = await factory.BindAsync(listeningEndPoint, source.Token);
        logger.LogInformation("Started listening for connections at port {Port}", listeningEndPoint.Port);

        var identifier = 0;

        while (!source.IsCancellationRequested)
        {
            try
            {
                var connection = await listener.AcceptAsync(source.Token);

                if (connection is not null)
                {
                    var client = new MinecraftClient(
                        loggerFactory.CreateLogger<MinecraftClient>(),
                        connection,
                        identifier);

                    clients[identifier] = client;
                    identifier++;

                    _ = ExecuteAsync(client);
                }
                else
                {
                    break;
                }
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
                    "Unexpected exception from client {Identifier}: \"{Message}\"",
                    client.Identifier,
                    exception.Message);
            }

            logger.LogDebug("Removing client {Identifier}", client.Identifier);
            clients.Remove(client.Identifier);
        }
    }

    private Task TickingAsync()
    {
        logger.LogInformation("Started ticking");
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (listener is not null)
        {
            await listener.DisposeAsync();
        }

        source.Dispose();

        var tasks = new Task[clients.Count];

        for (var index = 0; index < clients.Count; index++)
        {
            tasks[index] = clients[index].DisposeAsync().AsTask();
        }

        await Task.WhenAll(tasks);
    }
}