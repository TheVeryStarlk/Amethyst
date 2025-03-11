namespace Amethyst.Abstractions.Worlds;

public interface IGenerator
{
    public void Generate(IWorld world, IChunk chunk);
}