using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Worlds;

namespace Amethyst.Entities.Players;

internal sealed class Player(IClient client, Guid guid, GameMode gameMode, string username, World world) : Entity, IPlayer
{
    public IClient Client => client;

    public IWorld World => current;

    public Guid Guid => guid;

    public GameMode GameMode { get; set; } = gameMode;

    public string Username => username;

    public string? Locale { get; set; }

    public byte ViewDistance { get; set; }

    public Inventory Inventory { get; } = new();

    // The world this player is tracked by.
    private World current = world;

    public void Spawn(IWorld world, Position position)
    {
        if (World == world)
        {
            return;
        }

        if (world is not World destination)
        {
            throw new ArgumentException("The world was not created by an IWorldFactory.", nameof(world));
        }

        if (!destination.TryAdd(this))
        {
            throw new InvalidOperationException("The player is already in that world.");
        }

        current.Remove(this);
        current = destination;

        Client.Write(new RespawnPacket(this, World), new PositionLookPacket(position, Yaw, Pitch));
    }

    public void Teleport(Position position)
    {
        Position = position;
        Client.Write(new PositionLookPacket(position, Yaw, Pitch));
    }

    /// <summary>
    /// Stops the player's world from tracking it.
    /// </summary>
    public void Leave()
    {
        current.Remove(this);
    }
}