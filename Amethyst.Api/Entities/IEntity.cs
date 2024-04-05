using Amethyst.Api.Components;
using Amethyst.Api.Worlds;

namespace Amethyst.Api.Entities;

public interface IEntity
{
    public IServer Server { get; }

    public IWorld World { get; set; }

    public int Identifier { get; }

    public VectorF Position { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    public Task SpawnAsync();
}