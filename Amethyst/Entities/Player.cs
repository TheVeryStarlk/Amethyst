using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Messages;
using Amethyst.Components.Protocol;
using Amethyst.Components.Worlds;
using Amethyst.Protocol.Packets.Play;
using Amethyst.Worlds;

namespace Amethyst.Entities;

internal sealed class Player(Client client, string username) : IPlayer
{
    public IClient Client => client;

    public string Username => username;

    public Location Location { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    public IWorld? World => world;

    private readonly List<Position> chunks = [];

    private World? world;

    public void Update()
    {
        // Should be configurable via client packets.
        const int range = 2;

        var current = new Position((int) Location.X >> 4, 0, (int) Location.Z >> 4);
        var temporary = new List<Position>();

        for (var x = current.X - range; x < current.X + range; x++)
        {
            for (var z = current.Z - range; z < current.Z + range; z++)
            {
                temporary.Add(new Position(x, 0, z));
            }
        }

        var dead = chunks.Except(temporary).ToArray();

        foreach (var position in dead)
        {
            client.Write(new ChunkUnloadPacket(position.X, position.Z));
            chunks.Remove(position);
        }

        foreach (var position in temporary.Where(position => !chunks.Contains(position)))
        {
            chunks.Add(position);
            client.Write(world!.GetChunk(position).Build());
        }
    }

    public void Spawn(IWorld spawn)
    {
        if (world == spawn)
        {
            return;
        }

        IOutgoingPacket packet = World is null
            ? new JoinGamePacket(client.Identifier, 1, 0, 0, 1, "flat", false)
            : new RespawnPacket(0, 0, 1, "flat");

        client.Write(packet, new PositionLookPacket(Location, Yaw, Pitch, OnGround));

        world = (World) spawn;
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