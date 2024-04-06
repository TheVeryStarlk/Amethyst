using Amethyst.Api.Components;
using Amethyst.Api.Worlds;

namespace Amethyst.Api.Entities;

public interface IEntity
{
    public IServer Server { get; }

    public int Identifier { get; }

    public IWorld World { get; set; }

    public VectorF Vector { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }
}