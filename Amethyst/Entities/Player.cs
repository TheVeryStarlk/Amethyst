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

    public IWorld? World { get; set; }

    private readonly List<Position> chunks = [];

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

        var world = (World) World!;

        foreach (var position in temporary.Where(position => !chunks.Contains(position)))
        {
            chunks.Add(position);

            var chunk = world.GetChunk(position);

            for (var x = 0; x < 16; x++)
            {
                for (var z = 0; z < 16; z++)
                {
                    chunk.SetBlock(new Block(1), new Position(x, 2, z));
                }
            }

            client.Write(chunk.Build());
        }
    }

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