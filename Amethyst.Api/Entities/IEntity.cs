using Amethyst.Api.Worlds;

namespace Amethyst.Api.Entities;

public interface IEntity
{
    public IServer Server { get; }

    public IWorld World { get; set; }

    public int Identifier { get; }

    public Task SpawnAsync();
}