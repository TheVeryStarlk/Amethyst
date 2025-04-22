using System.Numerics;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Worlds;

namespace Amethyst.Entities.Players;

internal sealed class Player(IClient client, Guid guid, GameMode gameMode, string username, IWorld world) : Entity, IPlayer
{
    public IClient Client => client;

    public IWorld World { get; set; } = world;

    public Guid Guid => guid;

    public GameMode GameMode { get; set; } = gameMode;

    public string Username => username;

    public string? Locale { get; set; }

    public byte ViewDistance { get; set; }

    public Inventory Inventory { get; } = new();

    private readonly HashSet<long> chunks = [];

    public void Teleport(Position position)
    {
        Position = position;
        Client.Write(new PositionLookPacket(position, Yaw, Pitch));
    }

    public void Synchronize(Position position, float yaw, float pitch, bool ground)
    {
        Position = position;
        Yaw = yaw;
        Pitch = pitch;
        Ground = ground;

        var alive = new long[ViewDistance * ViewDistance * 4];
        var index = 0;

        var current = (X: ((int) Position.X).ToChunk(), Z: ((int) Position.Z).ToChunk());

        for (var x = current.X - ViewDistance; x < current.X + ViewDistance; x++)
        {
            for (var z = current.Z - ViewDistance; z < current.Z + ViewDistance; z++)
            {
                alive[index++] = NumericUtility.Encode(x, z);
            }
        }

        var dead = chunks.Except(alive).ToArray();

        foreach (var value in dead)
        {
            value.Decode(out var x, out var z);

            Client.Write(new SingleChunkPacket(x, z, [], 0));
            chunks.Remove(value);
        }

        var closest = alive.OrderBy(value =>
        {
            value.Decode(out var x, out var z);
            return Vector2.Distance(new Vector2(current.X, current.Z), new Vector2(x, z));
        });

        foreach (var value in closest)
        {
            if (!chunks.Add(value))
            {
                continue;
            }

            value.Decode(out var x, out var z);

            var result = World[x, z].Build();
            Client.Write(new SingleChunkPacket(x, z, result.Sections, result.Bitmask));
        }

        // Use relative movement packets.
        var packet = new EntityTeleportPacket(this);

        foreach (var pair in World.Players.Where(pair => pair.Value != this))
        {
            pair.Value.Client.Write(packet);
        }
    }
}