using Amethyst.Api;
using Amethyst.Api.Entities;

namespace Amethyst.Entities;

internal sealed class Player(IServer server, IClient client) : IPlayer
{
    public int Identifier { get; } = Random.Shared.Next();

    public IServer Server => server;

    public void Kick()
    {
        client.Stop();
    }
}