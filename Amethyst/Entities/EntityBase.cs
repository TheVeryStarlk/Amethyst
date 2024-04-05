using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Worlds;

namespace Amethyst.Entities;

internal abstract class EntityBase : IEntity
{
    public int Identifier { get; } = Random.Shared.Next();

    public required IServer Server { get; init; }

    public required IWorld World { get; set; }

    public VectorF Position { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }
}