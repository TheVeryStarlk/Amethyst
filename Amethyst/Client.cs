using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Channels;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Networking.Packets;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Networking;
using Amethyst.Networking.Packets;
using Amethyst.Networking.Serializers;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(ILogger<Client> logger, EventDispatching eventDispatching, Socket socket) : IClient, IDisposable
{
    public Player? Player { get; set; }

    public State State { get; set; }

    private readonly NetworkStream stream = new(socket);
    private readonly CancellationTokenSource source = new();
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

    public async Task StartAsync()
    {
        var reading = ReadingAsync();
        var writing = WritingAsync();

        if (await Task.WhenAny(reading, writing).ConfigureAwait(false) == reading)
        {
            outgoing.Writer.Complete();
            await writing.ConfigureAwait(false);
        }

        source.Cancel();
    }

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
                    packet.Create(State).Process(this, eventDispatching);
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