using System.IO.Pipelines;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Plugins.Events;
using Amethyst.Entities;
using Amethyst.Extensions;
using Amethyst.Networking;
using Amethyst.Networking.Packets.Handshaking;
using Amethyst.Networking.Packets.Login;
using Amethyst.Networking.Packets.Playing;
using Amethyst.Networking.Packets.Status;
using Amethyst.Plugins;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(
    ILogger<Client> logger,
    Server server,
    ConnectionContext connection,
    int identifier) : IAsyncDisposable
{
    public int Identifier => identifier;

    public Server Server => server;

    public IDuplexPipe Transport => connection.Transport;

    public Player? Player { get; private set; }

    public int KeepAliveCount { get; set; }

    private ClientState state;

    private readonly CancellationTokenSource source = new CancellationTokenSource();

    public async Task StartAsync()
    {
        while (!source.IsCancellationRequested)
        {
            try
            {
                var message = await Transport.Input.ReadMessageAsync(source.Token);

                if (message is null)
                {
                    break;
                }

                var task = state switch
                {
                    ClientState.Handshaking => HandleHandshakingAsync(message),
                    ClientState.Status => HandleStatusAsync(message),
                    ClientState.Login => HandleLoginAsync(message),
                    ClientState.Playing => HandlePlayingAsync(message),
                    ClientState.Disconnected => throw new OperationCanceledException(),
                    _ => throw new ArgumentOutOfRangeException()
                };

                await task;
            }
            catch (Exception exception) when (exception is OperationCanceledException or ConnectionResetException)
            {
                break;
            }
        }

        if (Player is not null)
        {
            Server.Status.PlayerInformation.Online--;

            var eventArgs = await Server.PluginService.ExecuteAsync(
                new PlayerLeaveEventArgs
                {
                    Server = Server,
                    Player = Player,
                    Message = ChatMessage.Create($"{Player.Username} has left the server.", Color.Yellow)
                });

            await Server.BroadcastChatMessageAsync(eventArgs.Message);
        }

        connection.Abort();
        logger.LogDebug("Client stopped {Identifier}", identifier);
    }

    public async Task StopAsync()
    {
        if (state is ClientState.Disconnected)
        {
            return;
        }

        state = ClientState.Disconnected;
        await source.CancelAsync();
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

        state = handshake.NextState;
        logger.LogDebug("Client switched state to {State}", state);
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

        Player = new Player(this, loginStart.Username)
        {
            GameMode = GameMode.Creative
        };

        await loginStart.HandleAsync(this);

        state = ClientState.Playing;
        logger.LogDebug("Login success with username: \"{Username}\"", Player.Username);
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

internal enum ClientState
{
    Handshaking,
    Status,
    Login,
    Playing,
    Disconnected
}