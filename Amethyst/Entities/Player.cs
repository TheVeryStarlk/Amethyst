using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Messages;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Entities;

internal sealed class Player(Server server, Client client, string username) : IPlayer
{
    public IServer Server => server;

    public int Identifier => client.Identifier;

    public IClient Client => client;

    public string Username => username;

    public ValueTask SendAsync(Message message, byte position)
    {
        return client.WriteAsync(new MessagePacket(message.Serialize(), position));
    }

    public ValueTask KeepAliveAsync()
    {
        return client.WriteAsync(new KeepAlivePacket(Random.Shared.Next()));
    }

    public void Disconnect(Message reason)
    {
        client.Stop(reason);
    }
}