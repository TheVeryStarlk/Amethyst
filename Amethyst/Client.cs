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

        var states = new Dictionary<State, Action<Packet>>
        {
            { State.Handshake, Handshake },
            { State.Status, Status },
            { State.Login, Login },
            { State.Play, Play }
        }.ToFrozenDictionary();

        while (true)
        {
            try
            {
                var packet = await reader.ReadAsync(source.Token).ConfigureAwait(false);
                states[state](packet);
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

        player = new Player(this, loginStart.Username);

        eventDispatcher.Dispatch(player, new Joining());

        // Quit before switching to play state if token was cancelled.
        source.Token.ThrowIfCancellationRequested();

        Write(new LoginSuccessPacket(Guid.NewGuid().ToString(), loginStart.Username));

        state = State.Play;

        eventDispatcher.Dispatch(player, new Joined());
    }

    private void Play(Packet packet)
    {
        if (Dispatchable.TryCreate(packet, out var dispatchable))
        {
            dispatchable.Dispatch(player!, eventDispatcher);
        }
    }
}

internal enum State
{
    Handshake,
    Status,
    Login,
    Play
}