using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Channels;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Eventing.Client;
using Amethyst.Abstractions.Packets;
using Amethyst.Entities.Players;
using Amethyst.Eventing;
using Amethyst.Networking;
using Amethyst.Networking.Packets;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(ILogger<Client> logger, Dispatcher dispatcher, Socket socket) : IClient, IDisposable
{
    public Player? Player { get; set; }

    public State State { get; set; }

    /// <summary>
    /// Set once when joining, packets at least this long are compressed.
    /// A negative threshold disables compression.
    /// </summary>
    public int Threshold { get; set; } = -1;

    /// <summary>
    /// The login waiting for the client to respond to the encryption request.
    /// </summary>
    public Verification? Verification { get; set; }

    private readonly CipherStream stream = new(new NetworkStream(socket));
    private readonly CancellationTokenSource source = new();
    private readonly Channel<Outgoing> outgoing = Channel.CreateUnbounded<Outgoing>();

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
            // Captured when queued, as the threshold is set before the earlier packets are written.
            if (!outgoing.Writer.TryWrite(new Outgoing(packet, Threshold)))
            {
                break;
            }
        }
    }

    public void Encrypt(byte[] secret)
    {
        stream.Encrypt(secret);
    }

    public void Stop()
    {
        source.Cancel();
    }

    public void Dispose()
    {
        source.Dispose();
        stream.Dispose();
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

                if (Protocol.TryRead(ref sequence, Threshold, out var packet))
                {
                    packet.Create(State).Process(this, dispatcher);
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

        await foreach (var item in outgoing.Reader.ReadAllAsync().ConfigureAwait(false))
        {
            try
            {
                Protocol.Write(output, item.Packet, item.Threshold);
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

internal sealed record Verification(Joining Joining, byte[] Token);

internal readonly struct Outgoing(IOutgoingPacket packet, int threshold)
{
    public IOutgoingPacket Packet => packet;

    public int Threshold => threshold;
}

internal enum State
{
    Handshake,
    Status,
    Login,
    Play
}