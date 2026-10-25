namespace Amethyst.Abstractions.Entities;

public interface ITameable : IAgeable
{
    public bool IsSitting { get; set; }

    public bool IsTamed { get; set; }

    public Guid? Owner { get; set; }
}