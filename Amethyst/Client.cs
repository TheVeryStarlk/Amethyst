using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Channels;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Networking.Packets;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Networking;
using Amethyst.Networking.Packets;
using Amethyst.Networking.Processors;
using Amethyst.Networking.Serializers;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(ILogger<Client> logger, EventDispatcher eventDispatcher, Socket socket) : IClient, IDisposable
{
    public EventDispatcher EventDispatcher => eventDispatcher;

    public Player? Player { get; set; }

    public State State { get; set; }

    private readonly NetworkStream stream = new(socket);
    private readonly CancellationTokenSource source = new();
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

    public void Write(params ReadOnlySpan<IOutgoingPacket> packets)
    {
        if (source.IsCancellationRequested)
        {
            return;
        }

        foreach (var packet in packets)
        {
            if (!outgoing.Writer.TryWrite(packet))
            {
                break;
            }
        }
    }

    public void Stop()
    {
        source.Cancel();
    }

    public void Dispose()
    {
        source.Dispose();
        socket.Dispose();
    }

    private async Task ReadingAsync()
    {
        var input = PipeReader.Create(stream);

        while (true)
        {
            try
            {
                var result = await input.ReadAsync(source.Token).ConfigureAwait(false);
                var sequence = result.Buffer;

                var consumed = sequence.Start;
                var examined = sequence.End;

                if (Protocol.TryRead(ref sequence, out var packet))
                {
                    Action<Client, Packet> action = State switch
                    {
                        State.Handshake => HandshakeProcessor.Process,
                        State.Status => StatusProcessor.Process,
                        State.Login => LoginProcessor.Process,
                        State.Play => PlayProcessor.Process,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    action(this, packet);

                    examined = consumed = sequence.Start;
                }

                if (result.IsCompleted)
                {
                    break;
                }

                input.AdvanceTo(consumed, examined);
            }
            catch (OperationCanceledException)
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
        var output = PipeWriter.Create(stream);

        // Serializers return the packet's length without accounting for the identifier and the total packet length,
        // which are both encoded as variable integers that take at most five bytes.
        const int extra = sizeof(int) + sizeof(int) + sizeof(short);

        await foreach (var packet in outgoing.Reader.ReadAllAsync().ConfigureAwait(false))
        {
            try
            {
                var serializer = packet.Create();
                var span = output.GetSpan(serializer.Length + extra);

                output.Advance(Protocol.Write(span, packet, serializer));
                await output.FlushAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An exception occurred while writing");
                break;
            }
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