using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Messages;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Entities;

internal sealed class Player(Client client, string username) : IPlayer
{
    public int Identifier => client.Identifier;

    public IClient Client => client;

    public string Username => username;

    public ValueTask SendAsync(Message message, byte position)
    {
        return client.WriteAsync(new MessagePacket(message.Serialize(), position));
    }

    public void Disconnect(Message reason)
    {
        client.Stop(reason);
    }
}