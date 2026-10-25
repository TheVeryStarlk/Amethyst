using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Worlds;

namespace Amethyst.Entities.Players;

internal sealed class Player(IClient client, World world, Guid guid, GameMode gameMode, string username) : Entity, IPlayer
{
    public IClient Client => client;

    public IWorld World => world;

    public Guid Guid => guid;

    public GameMode GameMode { get; set; } = gameMode;

    public string Username => username;

    public string? Locale { get; set; }

    public byte ViewDistance { get; set; }

    private World world = world;

    public void Spawn(IWorld destination, Position position)
    {
        if (World == destination)
        {
            return;
        }

        if (destination is not World instance)
        {
            throw new ArgumentException("The world was not created by an IWorldFactory.", nameof(destination));
        }

        if (!instance.TryAdd(this))
        {
            throw new InvalidOperationException("A player with the same username is already in that world.");
        }

        
        world.Remove(this);
        world = instance;

        Client.Write(new RespawnPacket(this, World), new PositionLookPacket(position, Yaw, Pitch));
    }

    public void Teleport(Position position)
    {
        Position = position;
        Client.Write(new PositionLookPacket(position, Yaw, Pitch));
    }

    /// <summary>
    /// Stops the player's world from tracking it and releases its username.
    /// </summary>
    public void Leave()
    {
        world.Leave(this);
    }
}