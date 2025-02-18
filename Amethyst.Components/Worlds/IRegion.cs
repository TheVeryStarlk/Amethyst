namespace Amethyst.Components.Worlds;

public interface IRegion
{
    public (int X, int Z) Position { get; }

    public IEnumerable<IChunk> Chunks { get; }
}