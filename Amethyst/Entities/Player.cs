using Amethyst.Components;
using Amethyst.Components.Messages;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Entities;

public sealed class Player(Server server, Client client, string username)
{
    public Server Server => server;

    public int Identifier => client.Identifier;

    public Client Client => client;

    public string Username => username;

    public ValueTask SendAsync(Message message, MessagePosition position)
    {
        return client.WriteAsync(new MessagePacket(message.Serialize(), (byte) position));
    }

    public ValueTask KeepAliveAsync()
    {
        return client.WriteAsync(new KeepAlivePacket(Random.Shared.Next()));
    }

    public void Disconnect(Message message)
    {
        client.Stop(message);
    }
}