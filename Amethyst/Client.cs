using System.Threading.Channels;
using Amethyst.Components;
using Amethyst.Components.Eventing.Sources.Client;
using Amethyst.Components.Messages;
using Amethyst.Eventing;
using Amethyst.Protocol;
using Amethyst.Protocol.Packets.Handshake;
using Amethyst.Protocol.Packets.Login;
using Amethyst.Protocol.Packets.Play;
using Amethyst.Protocol.Packets.Status;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(
    ILogger<Client> logger,
    ConnectionContext connection,
    EventDispatcher eventDispatcher,
    int identifier) : IClient, IAsyncDisposable
{
    public int Identifier => identifier;

    public State State { get; private set; }

    private const int Version = 47;

    private readonly (ProtocolReader Input, ProtocolWriter Output) protocol = connection.CreateProtocol();
    private readonly CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(connection.ConnectionClosed);
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

    private Message message = "No reason provided.";

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

    public void Stop(Message reason)
    {
        message = reason;
        source.Cancel();
    }

    public ValueTask DisposeAsync()
    {
        source.Dispose();
        return connection.DisposeAsync();
    }

    private async Task ReadingAsync()
    {
        while (true)
        {
            try
            {
                var packet = await protocol.Input.ReadAsync(source.Token).ConfigureAwait(false);

                switch (State)
                {
                    case State.Handshake:
                        packet.Out(out HandshakePacket handshake);

                        State = (State) handshake.State;

                        if (handshake.Version is not Version && State is State.Login)
                        {
                            // Eh, should probably cache this.
                            var reason = Message
                                .Create()
                                .Write("I'm still rocking 1.8!").Red()
                                .Build();

                            Stop(reason);
                        }

                        break;

                    case State.Status:
                        switch (packet.Identifier)
                        {
                            case 0:
                                packet.Out(out StatusRequestPacket _);

                                var request = await eventDispatcher
                                    .DispatchAsync(this, new StatusRequest(), source.Token)
                                    .ConfigureAwait(false);

                                Write(new StatusResponsePacket(request.Status.Serialize()));
                                break;

                            case 1:
                                packet.Out(out PingPacket ping);
                                Write(new PongPacket(ping.Magic));

                                break;
                        }

                        break;

                    case State.Login:
                        packet.Out(out LoginStartPacket loginStart);

                        await eventDispatcher
                            .DispatchAsync(this, new Joining(loginStart.Username), source.Token)
                            .ConfigureAwait(false);

                        // Stop was called, do not continue execution.
                        if (source.IsCancellationRequested)
                        {
                            break;
                        }

                        Write(new LoginSuccessPacket(Guid.NewGuid().ToString(), loginStart.Username));
                        State = State.Play;

                        break;

                    case State.Play:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                protocol.Input.Advance();
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
        }

        IOutgoingPacket bye = State is State.Login
            ? new LoginFailurePacket(message.Serialize())
            : new DisconnectPacket(message.Serialize());

        Write(bye);

        outgoing.Writer.Complete();
    }

    private async Task WritingAsync()
    {
        try
        {
            await foreach (var packet in outgoing.Reader.ReadAllAsync().ConfigureAwait(false))
            {
                await protocol.Output.WriteAsync(packet).ConfigureAwait(false);
            }
        }
        catch (Exception exception) when (exception is OperationCanceledException or ConnectionResetException)
        {
            // Nothing.
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected exception while writing to client");
        }
    }
}