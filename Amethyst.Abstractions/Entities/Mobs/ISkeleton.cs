namespace Amethyst.Abstractions.Entities.Mobs;

public interface ISkeleton : IMob
{
    public SkeletonType Type { get; set; }
}

public enum SkeletonType
{
    Normal,
    Wither
}