using Amethyst.Components;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amethyst;

internal sealed class MinecraftServer(MinecraftServerConfiguration configuration) : IMinecraftServer
{
    private const int ProtocolVersion = 47;

    private IConnectionListener? listener;

    private readonly ILogger<MinecraftServer> logger = configuration.LoggerFactory.CreateLogger<MinecraftServer>();
    private readonly CancellationTokenSource source = new CancellationTokenSource();
    private readonly Dictionary<int, MinecraftClient> clients = [];

    public Status Status
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public Task StartAsync()
    {
        if (listener is not null)
        {
            throw new InvalidOperationException("Server has already started.");
        }

        logger.LogDebug("Starting the server");
        return Task.WhenAll(ListeningAsync(), TickingAsync());
    }

    public void Shutdown()
    {
        logger.LogDebug("Shutting down the server");
        source.Cancel();
    }

    private async Task ListeningAsync()
    {
        var factory = new SocketTransportFactory(
            Options.Create(new SocketTransportOptions()),
            configuration.LoggerFactory);

        listener = await factory.BindAsync(configuration.ListeningEndPoint);
        logger.LogInformation("Listening for connection at {Port}", configuration.ListeningEndPoint.Port);

        var identifier = 0;

        while (!source.IsCancellationRequested)
        {
            try
            {
                var connection = await listener.AcceptAsync(source.Token);

                if (connection is not null)
                {
                    var client = new MinecraftClient(
                        configuration.LoggerFactory.CreateLogger<MinecraftClient>(),
                        connection);

                    clients[identifier++] = client;

                    _ = Task.Run(client.StartAsync);
                }
                else
                {
                    break;
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private Task TickingAsync()
    {
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (listener is not null)
        {
            await listener.DisposeAsync();
        }

        source.Dispose();

        foreach (var client in clients)
        {
            await client.Value.DisposeAsync();
        }
    }
}

/// <summary>
/// Represents a Minecraft server.
/// </summary>
internal interface IMinecraftServer : IAsyncDisposable
{
    /// <summary>
    /// Stores the server's status, contains player information and MOTD.
    /// </summary>
    public Status Status { get; }

    /// <summary>
    /// Stops listening for connections & shutdowns the server's tasks.
    /// </summary>
    public void Shutdown();
}