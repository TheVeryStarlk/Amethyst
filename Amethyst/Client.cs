using System.Threading.Channels;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Eventing.Clients;
using Amethyst.Abstractions.Eventing.Players;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Protocol;
using Amethyst.Protocol.Packets.Handshake;
using Amethyst.Protocol.Packets.Login;
using Amethyst.Protocol.Packets.Play;
using Amethyst.Protocol.Packets.Status;
using Amethyst.Worlds;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(ILogger<Client> logger, ConnectionContext connection, EventDispatcher eventDispatcher, int identifier)
    : IClient, IAsyncDisposable
{
    public int Identifier => identifier;

    public IPlayer? Player => player;

    private readonly CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(connection.ConnectionClosed);
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

    private State state;
    private Player? player;
    private Message reason = "No reason specified.";

    public Task StartAsync()
    {
        return Task.WhenAll(ReadingAsync(), WritingAsync());
    }

    public void Write(params ReadOnlySpan<IOutgoingPacket> packets)
    {
        foreach (var packet in packets)
        {
            if (!outgoing.Writer.TryWrite(packet))
            {
                break;
            }
        }
    }

    public void Stop(Message message)
    {
        reason = message;
        source.Cancel();
    }

    public ValueTask DisposeAsync()
    {
        source.Dispose();
        return connection.DisposeAsync();
    }

    private async Task ReadingAsync()
    {
        var reader = new ProtocolReader(connection.Transport.Input);

        while (true)
        {
            try
            {
                var packet = await reader.ReadAsync(source.Token).ConfigureAwait(false);

                switch (state)
                {
                    case State.Handshake:
                        Handshake(packet);
                        break;

                    case State.Status:
                        Status(packet);
                        break;

                    case State.Login:
                        Login(packet);
                        break;

                    case State.Play:
                        Play(packet);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception exception) when (exception is OperationCanceledException or ConnectionResetException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected exception while reading from client");
                break;
            }
            finally
            {
                reader.Advance();
            }
        }

        if (state is not State.Status)
        {
            IOutgoingPacket final = state is State.Login
                ? new LoginFailurePacket(reason.Serialize())
                : new DisconnectPacket(reason.Serialize());

            Write(final);

            if (state is State.Play)
            {
                eventDispatcher.Dispatch(player!, new Left());
            }
        }

        // Give client some time to realize the packets.
        await Task.Delay(50).ConfigureAwait(false);

        connection.Abort();
    }

    private async Task WritingAsync()
    {
        var writer = new ProtocolWriter(connection.Transport.Output);

        try
        {
            await foreach (var packet in outgoing.Reader.ReadAllAsync(connection.ConnectionClosed).ConfigureAwait(false))
            {
                await writer.WriteAsync(packet).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Nothing.
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected exception while writing to client");
        }
    }

    private void Handshake(Packet packet)
    {
        var handshake = packet.Create<HandshakePacket>();

        state = (State) handshake.State;

        if (state is State.Login or State.Play || handshake.Version is 47)
        {
            return;
        }

        var outdated = eventDispatcher.Dispatch(this, new Outdated(handshake.Version));
        Stop(outdated.Message);
    }

    private void Status(Packet packet)
    {
        if (packet.Identifier == StatusRequestPacket.Identifier)
        {
            var request = eventDispatcher.Dispatch(this, new Request());
            Write(new StatusResponsePacket(request.Status.Serialize()));

            return;
        }

        Write(packet.Create<PingPongPacket>());
        Stop(string.Empty);
    }

    private void Login(Packet packet)
    {
        var loginStart = packet.Create<LoginStartPacket>();
        var joining = eventDispatcher.Dispatch(this, new Joining(loginStart.Username));

        // Quit before switching to play state if token was cancelled.
        source.Token.ThrowIfCancellationRequested();

        if (joining.World is not World world)
        {
            logger.LogWarning("No joining world specified.");
            return;
        }

        player = new Player(this, loginStart.Username, world);

        Write(
            new LoginSuccessPacket(player.Guid.ToString(), loginStart.Username),
            new JoinGamePacket(Identifier, 1, 0, 0, 1, "flat", false),
            new PositionLookPacket(new Location(), 0, 0, false));

        state = State.Play;

        eventDispatcher.Dispatch(player, new Joined());
    }

    private void Play(Packet packet)
    {
        Dispatchable.Create(packet)?.Dispatch(player!, eventDispatcher);
    }
}

internal enum State
{
    Handshake,
    Status,
    Login,
    Play
}