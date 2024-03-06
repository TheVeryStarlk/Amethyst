using System.IO.Pipelines;
using Amethyst.Api.Components;
using Amethyst.Api.Plugin.Events;
using Amethyst.Entities;
using Amethyst.Networking;
using Amethyst.Networking.Packets.Handshaking;
using Amethyst.Networking.Packets.Login;
using Amethyst.Networking.Packets.Playing;
using Amethyst.Networking.Packets.Status;
using Amethyst.Plugin;
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

    public Player? Player { get; private set; }

    public CancellationToken CancellationToken => source.Token;

    private MinecraftClientState state;

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

            switch (state)
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
        if (state is MinecraftClientState.Disconnected)
        {
            return;
        }

        logger.LogDebug("Stopping client");

        await source.CancelAsync();
        connection.Abort();

        state = MinecraftClientState.Disconnected;
    }

    public async ValueTask DisposeAsync()
    {
        source.Dispose();
        await connection.DisposeAsync();
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

        state = handshake.NextState;
        logger.LogDebug("Client switched state to {State}", state);
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
        state = MinecraftClientState.Playing;
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

    public async Task HandleKeepAliveAsync()
    {
        if (state is not MinecraftClientState.Playing)
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
}

internal enum MinecraftClientState
{
    Handshaking,
    Status,
    Login,
    Playing,
    Disconnected
}