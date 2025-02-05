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

    public double X { get; internal set; }

    public double Y { get; internal set; }

    public double Z { get; internal set; }

    public float Yaw { get; internal set; }

    public float Pitch { get; internal set; }

    public bool OnGround { get; internal set; }

    public ValueTask MoveAsync(double x, double y, double z, float yaw, float pitch)
    {
        return client.WriteAsync(new PositionLookPacket(x, y, z, yaw, pitch, false));
    }

    public ValueTask SendAsync(Message message, MessagePosition position = MessagePosition.Box)
    {
        return client.WriteAsync(new MessagePacket(message.Serialize(), (byte) position));
    }

    public void Disconnect(Message message)
    {
        client.Stop(message);
    }
}