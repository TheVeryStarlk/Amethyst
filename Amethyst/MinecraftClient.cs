using Amethyst.Networking;
using Amethyst.Networking.Packets.Handshaking;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class MinecraftClient(
    ILogger<MinecraftClient> logger,
    ConnectionContext connection,
    int identifier) : IAsyncDisposable
{
    public int Identifier => identifier;

    private MinecraftClientState state;

    private readonly CancellationTokenSource source = new CancellationTokenSource();

    public async Task StartAsync()
    {
        while (!source.IsCancellationRequested)
        {
            var message = await connection.Transport.Input.ReadMessageAsync(source.Token);

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
                    break;

                case MinecraftClientState.Login:
                    break;

                case MinecraftClientState.Playing:
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
            || handshake.NextState is not (MinecraftClientState.Status or MinecraftClientState.Login))
        {
            logger.LogDebug("Not supported protocol version or invalid next state");
            await StopAsync();
        }

        state = handshake.NextState;
        logger.LogDebug("Client switched state to {State}", state);
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