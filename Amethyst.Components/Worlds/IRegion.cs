namespace Amethyst.Components.Worlds;

public interface IRegion
{
    public Position Position { get; }

    public IEnumerable<IChunk> Chunks { get; }
}