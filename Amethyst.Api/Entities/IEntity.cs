using Amethyst.Api.Components;
using Amethyst.Api.Levels;

namespace Amethyst.Api.Entities;

public interface IEntity
{
    public int Identifier { get; }

    public IWorld World { get; }

    public VectorF Position { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    public Task TeleportAsync(VectorF position);
}