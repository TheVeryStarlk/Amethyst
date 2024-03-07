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

    public MinecraftClientState State { get; private set; }

    public Player? Player { get; private set; }

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

            var task = State switch
            {
                MinecraftClientState.Handshaking => HandleHandshakingAsync(message),
                MinecraftClientState.Status => HandleStatusAsync(message),
                MinecraftClientState.Login => HandleLoginAsync(message),
                MinecraftClientState.Playing => HandlePlayingAsync(message),
                _ => throw new ArgumentOutOfRangeException()
            };

            await task;
        }

        var eventArgs = await Server.PluginService.ExecuteAsync(
            new PlayerLeaveEventArgs
            {
                Server = Server,
                Player = Player!,
                Message = ChatMessage.Create($"{Player!.Username} has left the server.", Color.Yellow)
            });

        await Server.BroadcastChatMessageAsync(eventArgs.Message);
    }

    public async Task StopAsync()
    {
        logger.LogInformation("Stopping client");
        State = MinecraftClientState.Disconnected;
        await source.CancelAsync();
        connection.Abort();
    }

    public async ValueTask DisposeAsync()
    {
        source.Dispose();
        await connection.DisposeAsync();
    }

    private async Task HandleHandshakingAsync(Message message)
    {
        var handshake = message.As<HandshakePacket>();
        await handshake.HandleAsync(this);

        State = handshake.NextState;
        logger.LogDebug("Client switched state to {State}", State);
    }

    private async Task HandleStatusAsync(Message message)
    {
        if (message.Identifier == StatusRequestPacket.Identifier)
        {
            await message.As<StatusRequestPacket>().HandleAsync(this);
            return;
        }

        if (message.Identifier == PingRequestPacket.Identifier)
        {
            await message.As<PingRequestPacket>().HandleAsync(this);
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
        await loginStart.HandleAsync(this);

        logger.LogDebug("Login success with username: \"{Username}\"", Player.Username);
        State = MinecraftClientState.Playing;
    }

    private async Task HandlePlayingAsync(Message message)
    {
        var task = message.Identifier switch
        {
            0x00 => message.As<KeepAlivePacket>().HandleAsync(this),
            0x01 => message.As<ChatMessagePacket>().HandleAsync(this),
            0x03 => message.As<OnGroundPacket>().HandleAsync(this),
            0x04 => message.As<PlayerPositionPacket>().HandleAsync(this),
            0x05 => message.As<PlayerLookPacket>().HandleAsync(this),
            0x06 => message.As<PlayerPositionAndLookPacket>().HandleAsync(this),
            _ => Task.CompletedTask
        };

        await task;
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