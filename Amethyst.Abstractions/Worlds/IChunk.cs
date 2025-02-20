namespace Amethyst.Abstractions.Worlds;

public interface IChunk
{
    public Block GetBlock(Position position);

    public void SetBlock(Block block, Position position);

    public Biome GetBiome(int x, int z);

    public void SetBiome(Biome biome, int x, int z);

    /// <summary>
    /// Serializes the <see cref="IChunk"/>'s sections and encodes a bitmask for it.
    /// </summary>
    /// <remarks>
    /// This is intended for internal usage.
    /// </remarks>
    /// <returns>The serialized sections and their bitmask</returns>
    public (byte[] Sections, ushort Bitmask) Build();
}