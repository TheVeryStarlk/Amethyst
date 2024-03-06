using System.IO.Pipelines;
using Amethyst.Api.Components;
using Amethyst.Api.Plugins.Events;
using Amethyst.Entities;
using Amethyst.Networking;
using Amethyst.Networking.Packets.Handshaking;
using Amethyst.Networking.Packets.Login;
using Amethyst.Networking.Packets.Playing;
using Amethyst.Networking.Packets.Status;
using Amethyst.Plugins;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class MinecraftClient(
    ILogger<MinecraftClient> logger,
    MinecraftServer server,
    ConnectionContext connection,
    int identifier) : IAsyncDisposable
{
    public int Identifier => identifier;

    public MinecraftServer Server => server;

    public IDuplexPipe Transport => connection.Transport;

    public CancellationToken CancellationToken => source.Token;

    public Player? Player { get; private set; }

    public MinecraftClientState State { get; private set; }

    private readonly CancellationTokenSource source = new CancellationTokenSource();

    public async Task StartAsync()
    {
        while (!source.IsCancellationRequested)
        {
            var message = await Transport.Input.ReadMessageAsync(CancellationToken);

            if (message is null)
            {
                break;
            }

            switch (State)
            {
                case MinecraftClientState.Handshaking:
                    await HandleHandshakingAsync(message);
                    break;

                case MinecraftClientState.Status:
                    await HandleStatusAsync(message);
                    break;

                case MinecraftClientState.Login:
                    await HandleLoginAsync(message);
                    break;

                case MinecraftClientState.Playing:
                    await HandlePlayingAsync(message);
                    break;

                case MinecraftClientState.Disconnected:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public async Task StopAsync()
    {
        if (State is MinecraftClientState.Disconnected)
        {
            return;
        }

        State = MinecraftClientState.Disconnected;

        logger.LogDebug("Stopping client");
        await source.CancelAsync();
        connection.Abort();
    }

    public async ValueTask DisposeAsync()
    {
        source.Dispose();
        await connection.DisposeAsync();
    }

    public async Task HandleKeepAliveAsync()
    {
        if (State is not MinecraftClientState.Playing)
        {
            return;
        }

        await Transport.Output.WritePacketAsync(
            new KeepAlivePacket
            {
                Payload = Random.Shared.Next()
            },
            CancellationToken);
    }

    private async Task HandleHandshakingAsync(Message message)
    {
        var handshake = message.As<HandshakePacket>();

        if (handshake.ProtocolVersion != MinecraftServer.ProtocolVersion
            && handshake.NextState is MinecraftClientState.Login)
        {
            logger.LogDebug("Not supported protocol version");

            await Transport.Output.WritePacketAsync(
                new DisconnectPacket(MinecraftClientState.Login)
                {
                    Reason = ChatMessage.Create(
                        handshake.ProtocolVersion > MinecraftServer.ProtocolVersion
                            ? "Outdated server"
                            : "Outdated client",
                        Color.Red)
                },
                CancellationToken);

            await StopAsync();
            return;
        }

        State = handshake.NextState;
        logger.LogDebug("Client switched state to {State}", State);
    }

    private async Task HandleStatusAsync(Message message)
    {
        if (message.Identifier == StatusRequestPacket.Identifier)
        {
            var eventArgs = await server.PluginService.ExecuteAsync(new DescriptionRequestedEventArgs
            {
                Server = server,
                Description = server.Status.Description
            });

            await server.PluginService.ExecuteEventAsync(eventArgs);
            server.Status.Description = eventArgs.Description;

            await Transport.Output.WritePacketAsync(
                new StatusResponsePacket
                {
                    Status = server.Status
                },
                CancellationToken);

            return;
        }

        if (message.Identifier == PingRequestPacket.Identifier)
        {
            var ping = message.As<PingRequestPacket>();

            await Transport.Output.WritePacketAsync(
                new PongResponsePacket
                {
                    Payload = ping.Payload
                },
                CancellationToken);
        }
        else
        {
            logger.LogDebug("Unknown packet while handling status");
        }

        await StopAsync();
    }

    private async Task HandleLoginAsync(Message message)
    {
        var loginStart = message.As<LoginStartPacket>();

        Player = new Player(this, loginStart.Username);

        await Transport.Output.WritePacketAsync(
            new LoginSuccessPacket
            {
                Guid = Player.Guid,
                Username = Player.Username
            },
            CancellationToken);

        logger.LogDebug("Login success with username: \"{Username}\"", Player.Username);
        State = MinecraftClientState.Playing;
        await Player.JoinAsync();
    }

    private async Task HandlePlayingAsync(Message message)
    {
        switch (message.Identifier)
        {
            case 0x01:
                await message.As<ChatMessagePacket>().HandleAsync(this);
                break;
        }
    }
}

internal enum MinecraftClientState
{
    Handshaking,
    Status,
    Login,
    Playing,
    Disconnected
}