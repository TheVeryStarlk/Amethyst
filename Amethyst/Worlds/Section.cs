using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class Section
{
    private readonly byte[] blocks = new byte[8192];

    public Block GetBlock(int x, int y, int z)
    {
        var index = NumericUtility.AsIndex(x, y, z) * 2;

        var type = blocks[index] >> 4 | blocks[index + 1] << 4;
        var metadata = blocks[index] & 0x0F;

        return new Block(type, metadata);
    }

    public void SetBlock(Block block, int x, int y, int z)
    {
        var index = NumericUtility.AsIndex(x, y, z) * 2;
        var type = block.Type;

        blocks[index] = (byte) (type << 4 | block.Metadata);
        blocks[index + 1] = (byte) (type >> 4);
    }

    public byte[] Build()
    {
        // Lighting is not implemented "yet".
        return blocks;
    }
}