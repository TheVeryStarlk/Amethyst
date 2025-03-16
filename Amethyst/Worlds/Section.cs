using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class Section
{
    private readonly byte[] blocks = new byte[8192];

    public Block this[int x, int y, int z]
    {
        get
        {
            var index = AsIndex(x, y, z) * 2;

            var type = blocks[index] >> 4 | blocks[index + 1] << 4;
            var metadata = blocks[index] & 0x0F;

            return new Block(type, metadata);
        }
        set
        {
            var index = AsIndex(x, y, z) * 2;
            var type = value.Type;

            blocks[index] = (byte) (type << 4 | value.Metadata);
            blocks[index + 1] = (byte) (type >> 4);
        }
    }

    public byte[] Build()
    {
        // Lighting is not implemented "yet".
        return blocks;
    }

    private static int AsIndex(int x, int y, int z)
    {
        return (y & 0xF) << 8 | (z & 0xF) << 4 | x & 0xF;
    }
}