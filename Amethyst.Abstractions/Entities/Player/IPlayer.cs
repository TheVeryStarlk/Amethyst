using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Entities.Player;

public interface IPlayer : IEntity
{
    public IClient Client { get; }

    public IWorld World { get; }

    public string Unique { get; }

    public GameMode GameMode { get; }

    public string Username { get; }

    public string? Locale { get; }

    public byte ViewDistance { get; }
}