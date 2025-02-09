using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Messages;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Entities;

internal sealed class Player(Client client, string username) : IPlayer
{
    public IClient Client => client;

    public string Username => username;

    public double X { get; set; }

    public double Y { get; set; }

    public double Z { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    public void Move(double x, double y, double z, float pitch, float yaw)
    {
        X = x;
        Y = y;
        Z = z;
        Yaw = yaw;
        Pitch = pitch;

        client.Write(new PositionLookPacket(x, y, z, yaw, pitch, OnGround));
    }

    public void Send(Message message, MessagePosition position = MessagePosition.Box)
    {
        client.Write(new MessagePacket(message.Serialize(), (byte) position));
    }

    public void Disconnect(Message message)
    {
        client.Stop(message);
    }
}