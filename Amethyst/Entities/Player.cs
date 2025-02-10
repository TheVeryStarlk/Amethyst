using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Messages;
using Amethyst.Components.Protocol;
using Amethyst.Components.Worlds;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Entities;

internal sealed class Player(Client client, string username) : IPlayer
{
    public IClient Client => client;

    public string Username => username;

    public Location Location { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    public IWorld? World { get; set; }

    public void Spawn(IWorld world)
    {
        if (World == world)
        {
            return;
        }

        IOutgoingPacket packet = World is null
            ? new JoinGamePacket(client.Identifier, 1, 0, 0, 1, "flat", false)
            : new RespawnPacket(0, 0, 1, "flat");

        client.Write(packet, new PositionLookPacket(Location, Yaw, Pitch, OnGround));

        World = world;
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