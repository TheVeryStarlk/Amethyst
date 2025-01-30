using System.Threading.Channels;
using Amethyst.Components;
using Amethyst.Components.Eventing.Sources.Client;
using Amethyst.Components.Messages;
using Amethyst.Eventing;
using Amethyst.Protocol;
using Amethyst.Protocol.Packets.Handshake;
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

    private readonly CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(connection.ConnectionClosed);
    private readonly (ProtocolReader Input, ProtocolWriter Output) protocol = connection.CreateProtocol();
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

    private Message reason = "No reason provided.";

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
                        break;

                    case State.Status:
                        switch (packet.Identifier)
                        {
                            case 0:
                                packet.Out(out StatusRequestPacket _);

                                var request = await eventDispatcher.DispatchAsync(this, new StatusRequest(), source.Token).ConfigureAwait(false);

                                Write(new StatusResponsePacket
                                {
                                    Message = request.Status.Serialize()
                                });

                                break;

                            case 1:
                                packet.Out(out PingPacket ping);

                                Write(new PongPacket
                                {
                                    Magic = ping.Magic
                                });

                                break;
                        }

                        break;

                    case State.Login:
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
    }

    private async Task WritingAsync()
    {
        try
        {
            await foreach (var packet in outgoing.Reader.ReadAllAsync(source.Token).ConfigureAwait(false))
            {
                await protocol.Output.WriteAsync(packet, source.Token).ConfigureAwait(false);
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