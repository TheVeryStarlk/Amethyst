namespace Amethyst.Api.Levels;

public interface IRegion
{
    public IEnumerable<IChunk> Chunks { get; }
}