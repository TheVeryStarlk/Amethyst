using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Channels;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets;
using Amethyst.Eventing;
using Amethyst.Networking;
using Amethyst.Networking.Packets;
using Amethyst.Networking.Serializers;
using Microsoft.Extensions.Logging;

namespace Amethyst;

// Rewrite this when https://github.com/davidfowl/BedrockFramework/issues/172.
internal sealed class Client(ILogger<Client> logger, Socket socket, EventDispatcher eventDispatcher) : IClient, IDisposable
{
    public EventDispatcher EventDispatcher => eventDispatcher;

    // Probably shouldn't use random.
    public int Identifier { get; } = Random.Shared.Next();

    public IPlayer? Player { get; set; }

    public State State { get; set; }

    private readonly NetworkStream stream = new(socket);
    private readonly CancellationTokenSource source = new();
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

    public Task StartAsync()
    {
        return Task.WhenAll(ReadingAsync(), WritingAsync());
    }

    public void Write(IOutgoingPacket packet)
    {
        if (!outgoing.Writer.TryWrite(packet))
        {
            // Worth it to log that?
            logger.LogDebug("Failed to write {Name}", packet.GetType().Name);
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
        var reader = PipeReader.Create(stream);

        while (true)
        {
            var result = await reader.ReadAsync(source.Token).ConfigureAwait(false);
            var buffer = result.Buffer;

            var consumed = buffer.Start;
            var examined = buffer.End;

            try
            {
                if (TryRead(ref buffer, out var packet))
                {
                    consumed = buffer.Start;
                    examined = consumed;

                    var dictionary = State switch
                    {
                        State.Handshake => IngoingPacket.Handshake,
                        State.Status => IngoingPacket.Status,
                        State.Login => IngoingPacket.Login,
                        State.Play => IngoingPacket.Play,
                        _ => throw new ArgumentOutOfRangeException(nameof(State), State, "Unknown state.")
                    };

                    var span = packet.Sequence.IsSingleSegment ? packet.Sequence.First.Span : packet.Sequence.ToArray();
                    dictionary[packet.Identifier](span).Process(this);
                }

                if (result.IsCompleted)
                {
                    break;
                }
            }
            finally
            {
                reader.AdvanceTo(consumed, examined);
            }
        }

        return;

        static bool TryRead(ref ReadOnlySequence<byte> buffer, out Packet packet)
        {
            var reader = new SequenceReader<byte>(buffer);
            packet = default;

            if (!reader.TryReadVariableInteger(out var length) || !reader.TryReadVariableInteger(out var identifier))
            {
                return false;
            }

            var left = Variable.GetByteCount(identifier);

            if (!reader.TryReadExact(length - left, out var sequence))
            {
                return false;
            }

            packet = new Packet(identifier, sequence);
            buffer = buffer.Slice(sequence.End);

            return true;
        }
    }

    private async Task WritingAsync()
    {
        var output = PipeWriter.Create(stream);

        await foreach (var packet in outgoing.Reader.ReadAllAsync().ConfigureAwait(false))
        {
            try
            {
                var identifier = Variable.GetByteCount(packet.Identifier);
                var serializer = packet.Serializer();

                var length = identifier + serializer.Length;
                var body = Variable.GetByteCount(length);

                var total = body + length;
                var span = output.GetSpan(total);

                SpanWriter
                    .Create(span)
                    .WriteVariableInteger(length)
                    .WriteVariableInteger(packet.Identifier);

                serializer.Write(span[(body + identifier)..]);

                output.Advance(total);

                await output.FlushAsync(source.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An exception occured during writing.");
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