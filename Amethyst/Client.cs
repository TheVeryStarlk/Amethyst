﻿using Amethyst.Api;
using Amethyst.Api.Entities;
using Amethyst.Api.Plugins.Events.Server;
using Amethyst.Components;
using Amethyst.Entities;
using Amethyst.Extensions;
using Amethyst.Protocol;
using Amethyst.Protocol.Packets.Handshaking;
using Amethyst.Protocol.Packets.Status;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(
    ILogger<IClient> logger,
    int identifier,
    IServer server,
    ConnectionContext connection) : IClient, IAsyncDisposable
{
    private enum State
    {
        Handshaking,
        Status,
        Login,
        Playing,
        Disconnected
    }

    public IPlayer? Player { get; private set; }

    public int Identifier { get; } = identifier;

    private CancellationTokenSource? source;
    private DateTimeOffset idle;
    private State state;

    private readonly Queue<IOutgoingPacket> queue = [];
    private readonly Transport transport = new Transport(connection.Transport);

    public async Task StartAsync()
    {
        source = new CancellationTokenSource();
        state = State.Handshaking;

        logger.LogDebug("Started connection");

        while (true)
        {
            try
            {
                var message = await transport.ReadAsync(source.Token);

                if (message is null)
                {
                    break;
                }

                var task = state switch
                {
                    State.Handshaking => HandleHandshakingAsync(message),
                    State.Status => HandleStatusAsync(message),
                    State.Login => HandleLoginAsync(message),
                    State.Playing => HandlePlayingAsync(message),
                    State.Disconnected => Task.CompletedTask,
                    _ => throw new ArgumentOutOfRangeException()
                };

                await task;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    "Unexpected exception while handling packets: \"{Message}\"",
                    exception);

                break;
            }
        }

        state = State.Disconnected;

        if (Player is not null)
        {
            await Player.KickAsync();
        }

        connection.Abort();
    }

    public async Task TickAsync()
    {
        if (Player is null)
        {
            return;
        }

        if (DateTimeOffset.Now.Subtract(idle) > server.Options.IdleTimeOut)
        {
            await Player.KickAsync();
        }

        // Hmm, I wonder if order matters.
        await queue
            .Select(packet => transport.WriteAsync(packet))
            .WhenEach();
    }

    public void Queue(IOutgoingPacket packet)
    {
        queue.Enqueue(packet);
    }

    public void Stop()
    {
        source?.Cancel();
    }

    public async ValueTask DisposeAsync()
    {
        source?.Dispose();
        await connection.DisposeAsync();
    }

    private Task HandleHandshakingAsync(Message message)
    {
        var handshake = message.As<HandshakePacket>();

        state = (State) handshake.NextState;

        if (handshake.ProtocolVersion != server.ProtocolVersion)
        {
            Stop();
        }

        return Task.CompletedTask;
    }

    private async Task HandleStatusAsync(Message message)
    {
        switch (message.Identifier)
        {
            case 0x00:
                _ = message.As<StatusRequestPacket>();

                var request = new ServerDescriptionRequestEvent
                {
                    Server = server,
                    Description = server.Options.Description
                };

                await server.EventService.ExecuteAsync(request);

                await transport.WriteAsync(
                    new StatusResponsePacket
                    {
                        Status = ServerStatus.Create(
                            nameof(Amethyst),
                            server.ProtocolVersion,
                            server.Options.MaximumPlayers,
                            server.Players.Count(),
                            request.Description)
                    });

                return;

            case 0x01:
            {
                var ping = message.As<PingRequestPacket>();

                await transport.WriteAsync(
                    new PongResponsePacket
                    {
                        Payload = ping.Payload
                    });

                break;
            }
        }

        Stop();
    }

    private Task HandleLoginAsync(Message message)
    {
        Player = new Player(server, server.Worlds.Values.First(), this);
        return Task.CompletedTask;
    }

    private Task HandlePlayingAsync(Message message)
    {
        return Task.CompletedTask;
    }
}

internal interface IClient
{
    public IPlayer? Player { get; }

    public void Queue(IOutgoingPacket packet);

    public void Stop();
}