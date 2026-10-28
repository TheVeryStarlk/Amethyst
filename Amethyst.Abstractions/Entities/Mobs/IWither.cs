namespace Amethyst.Abstractions.Entities.Mobs;

public interface IWither : IMob
{
    /// <summary>
    /// The entity the center head looks at.
    /// </summary>
    public IEntity? CenterTarget { get; set; }

    public IEntity? RightTarget { get; set; }

    public IEntity? LeftTarget { get; set; }

    /// <summary>
    /// The ticks left until the wither stops being blue and growing, like it does after being built.
    /// </summary>
    public int InvulnerableTime { get; set; }
}