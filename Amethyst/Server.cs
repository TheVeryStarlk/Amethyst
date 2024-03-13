﻿using System.Net;
using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Extensions;
using Amethyst.Networking.Packets.Playing;
using Amethyst.Plugins;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Server(
    ServerConfiguration configuration,
    IConnectionListenerFactory listenerFactory,
    ILoggerFactory loggerFactory,
    PluginService pluginService,
    CancellationToken cancellationToken) : IServer
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

    private readonly ILogger<Server> logger = loggerFactory.CreateLogger<Server>();
    private readonly Dictionary<int, Client> clients = [];

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

        await pluginService.DisposeAsync();

        await clients.Values
            .Select(client => client.DisposeAsync().AsTask())
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

    private async Task ListeningAsync()
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
        await listener.UnbindAsync();
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

    private async Task TickingAsync()
    {
        logger.LogInformation("Started ticking");

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(50));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                foreach (var client in clients.Values.ToArray().Where(client => client.Player is not null))
                {
                    if (client.KeepAliveCount > configuration.MaximumMissedKeepAliveCount)
                    {
                        await DisconnectPlayerAsync(client.Player!, ChatMessage.Create("Timed out.", Color.Red));
                        continue;
                    }

                    await client.Transport.Output.WritePacketAsync(
                        new KeepAlivePacket
                        {
                            Payload = Random.Shared.Next()
                        });

                    client.KeepAliveCount++;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore.
        }

        logger.LogCritical("Stopped ticking");

        var reason = ChatMessage.Create("Server stopped.", Color.Red);

        await Players
            .Select(player => DisconnectPlayerAsync(player, reason))
            .WhenAll();
    }
}