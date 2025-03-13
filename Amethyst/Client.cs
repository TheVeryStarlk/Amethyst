using System.Net.Sockets;
using System.Threading.Channels;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Networking;
using Amethyst.Entities;
using Amethyst.Eventing;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(ILogger<Client> logger, Socket socket, EventDispatcher eventDispatcher) : IClient, IDisposable
{
    // Probably shouldn't use random.
    public int Identifier { get; } = Random.Shared.Next();

    public IPlayer Player => player ?? throw new InvalidOperationException();

    private Player? player;

    private readonly CancellationTokenSource source = new();
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

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