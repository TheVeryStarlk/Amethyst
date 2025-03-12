using System.Net.Sockets;
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

    public void Write(IOutgoingPacket packet)
    {
        throw new NotImplementedException();
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