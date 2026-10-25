using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities;

internal abstract class Spawnable(IWorld world) : Entity, ISpawnable
{
    public override IWorld World => world;

    public Velocity Velocity { get; set; }

    public bool IsOnFire { get; set; }

    public bool IsCrouched { get; set; }

    public bool IsSprinting { get; set; }

    public bool IsEating { get; set; }

    public bool IsInvisible { get; set; }

    public short Air { get; set; } = 300;

    public string? CustomName { get; set; }

    public bool IsCustomNameVisible { get; set; }

    public bool IsSilent { get; set; }
}