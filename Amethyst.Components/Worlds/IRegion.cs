namespace Amethyst.Components.Worlds;

public interface IRegion
{
    public int X { get; }

    public int Z { get; }

    public IEnumerable<IChunk> Chunks { get; }
}