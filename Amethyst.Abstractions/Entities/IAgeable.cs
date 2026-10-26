namespace Amethyst.Abstractions.Entities;

public interface IAgeable : IMob
{
    public bool IsChild { get; set; }
}