namespace Amethyst.Abstractions.Worlds;

public interface IChunk
{
    public Block GetBlock(Position position);

    public void SetBlock(Block block, Position position);

    public Biome GetBiome(int x, int z);

    public void SetBiome(Biome biome, int x, int z);

    public (byte[] Sections, ushort Bitmask) Build();
}