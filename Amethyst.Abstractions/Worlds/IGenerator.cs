namespace Amethyst.Abstractions.Worlds;

public interface IGenerator
{
    public void Generate(IChunk chunk);
}