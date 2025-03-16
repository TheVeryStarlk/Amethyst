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

    // Probably shouldn't use random.
    public int Identifier { get; } = Random.Shared.Next();

    public Player? Player { get; }

    private readonly CancellationTokenSource source = new();
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

    private State state;

    public void Write(IOutgoingPacket packet)
    {
        if (!outgoing.Writer.TryWrite(packet))
        {
            // Worth it to log that?
        }
    }

    public void Stop()
    {
        source.Cancel();
    }

    public void Dispose()
    {
        source.Dispose();
    }
}

internal enum State
{
    Handshake,
    Status,
    Login,
    Play
}