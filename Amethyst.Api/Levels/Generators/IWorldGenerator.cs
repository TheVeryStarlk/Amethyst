namespace Amethyst.Api.Levels.Generators;

public interface IWorldGenerator
{
    public void GenerateChunk(IChunk chunk);
}