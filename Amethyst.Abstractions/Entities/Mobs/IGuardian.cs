namespace Amethyst.Abstractions.Entities.Mobs;

public interface IGuardian : IMob
{
    public bool IsElder { get; set; }

    public bool IsRetractingSpikes { get; set; }

    /// <summary>
    /// The entity the guardian aims its beam at.
    /// </summary>
    public IEntity? Target { get; set; }
}