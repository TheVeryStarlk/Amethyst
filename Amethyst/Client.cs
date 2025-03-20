using System.Net.Sockets;
using System.Threading.Channels;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Networking.Packets;
using Amethyst.Entities;
using Amethyst.Eventing;
using Microsoft.Extensions.Logging;

namespace Amethyst;

// Rewrite this when https://github.com/davidfowl/BedrockFramework/issues/172.
internal sealed class Client(ILogger<Client> logger, Socket socket, EventDispatcher eventDispatcher) : IClient, IDisposable
{
    public EventDispatcher EventDispatcher => eventDispatcher;

    public Player? Player { get; }

    private readonly CancellationTokenSource source = new();
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

    private State state;

    public async Task StartAsync()
    {
        var reading = ReadingAsync();
        var writing = WritingAsync();

        // If reading finishes first, then either the client stopped the connection or the token was cancelled,
        // but there still is some work to do in the writing task, so wait for that work to finish.
        if (await Task.WhenAny(reading, writing).ConfigureAwait(false) == reading)
        {
            if (!source.IsCancellationRequested)
            {
                outgoing.Writer.Complete();
            }

            await writing.ConfigureAwait(false);
        }

        // Writing might finish before reading, so in that case cancelling the token will stop
        // the reading task thus marking the end of the entire reading writing process.
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
            // Writer complete is never called. Doesn't hurt to have this check, though.
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
        while (true)
        {
            try
            {
                await socket.ReceiveAsync(ArraySegment<byte>.Empty, source.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Exception while reading");
                break;
            }
        }
    }

    private async Task WritingAsync()
    {
        await foreach (var packet in outgoing.Reader.ReadAllAsync().ConfigureAwait(false))
        {
            try
            {
                // Work...
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Exception while writing");
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