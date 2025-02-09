using System.Collections.Frozen;
using System.Threading.Channels;
using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Eventing.Sources.Clients;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Messages;
using Amethyst.Components.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Protocol;
using Amethyst.Protocol.Packets.Handshake;
using Amethyst.Protocol.Packets.Login;
using Amethyst.Protocol.Packets.Play;
using Amethyst.Protocol.Packets.Status;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(ILogger<Client> logger, ConnectionContext connection, EventDispatcher eventDispatcher, int identifier)
    : IClient, IAsyncDisposable
{
    public int Identifier => identifier;

    public IPlayer? Player { get; private set; }

    private readonly CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(connection.ConnectionClosed);
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

    private State state;
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

                var task = state switch
                {
                    State.Handshake => HandshakeAsync(packet),
                    State.Status => StatusAsync(packet),
                    State.Login => LoginAsync(packet),
                    State.Play => PlayAsync(packet),
                    _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Invalid state.")
                };

                await task.ConfigureAwait(false);
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
                await eventDispatcher.DispatchAsync(Player!, new Left(), source.Token).ConfigureAwait(false);
            }
        }

        // Give client some time to realize the packets.
        await Task.Delay(50).ConfigureAwait(false);

        outgoing.Writer.Complete();
        connection.Abort();
    }

    private async Task WritingAsync()
    {
        var writer = new ProtocolWriter(connection.Transport.Output);

        try
        {
            await foreach (var packet in outgoing.Reader.ReadAllAsync().ConfigureAwait(false))
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

    private async Task HandshakeAsync(Packet packet)
    {
        var handshake = packet.Create<HandshakePacket>();

        state = (State) handshake.State;

        if (state is not State.Login || handshake.Version is 47)
        {
            return;
        }

        var outdated = await eventDispatcher.DispatchAsync(this, new Outdated(handshake.Version), source.Token).ConfigureAwait(false);
        Stop(outdated.Message);
    }

    private async Task StatusAsync(Packet packet)
    {
        if (packet.Identifier == StatusRequestPacket.Identifier)
        {
            var request = await eventDispatcher.DispatchAsync(this, new Request(), source.Token).ConfigureAwait(false);
            Write(new StatusResponsePacket(request.Status.Serialize()));

            return;
        }

        Write(packet.Create<PingPongPacket>());
        Stop(string.Empty);
    }

    private async Task LoginAsync(Packet packet)
    {
        var loginStart = packet.Create<LoginStartPacket>();

        Player = new Player(this, loginStart.Username);

        await eventDispatcher.DispatchAsync(Player, new Joining(), source.Token).ConfigureAwait(false);

        // Quit before switching to play state if token was cancelled.
        source.Token.ThrowIfCancellationRequested();

        Write(
            new LoginSuccessPacket(Guid.NewGuid().ToString(), loginStart.Username),
            new JoinGamePacket(Identifier, 0, 0, 0, 1, "default", false),
            new PositionLookPacket(0, 0, 0, 0, 0, false));

        state = State.Play;
    }

    private async Task PlayAsync(Packet packet)
    {
        var dictionary = new Dictionary<int, IPublisher>
        {
            { MessagePacket.Identifier, new MessagePacket(string.Empty, 0) },
            { OnGroundPacket.Identifier, new OnGroundPacket(false) },
            { PositionPacket.Identifier, new PositionPacket(0, 0, 0, false) },
            { LookPacket.Identifier, new LookPacket(0, 0, false) },
            { PositionLookPacket.Identifier, new PositionLookPacket(0, 0, 0, 0, 0, false) }
        }.ToFrozenDictionary();

        if (!dictionary.TryGetValue(packet.Identifier, out var publisher))
        {
            return;
        }

        await publisher.PublishAsync(packet, Player!, eventDispatcher, source.Token).ConfigureAwait(false);
    }
}

internal enum State
{
    Handshake,
    Status,
    Login,
    Play
}