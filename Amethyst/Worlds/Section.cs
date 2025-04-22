using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class Section
{
    private readonly byte[] blocks = new byte[8192];

    public Block this[int x, int y, int z]
    {
        get
        {
            var index = NumericUtility.AsIndex(x, y, z) * 2;

            var type = blocks[index] >> 4 | blocks[index + 1] << 4;
            var metadata = blocks[index] & 0x0F;

            return new Block(type, metadata);
        }
        set
        {
            var index = NumericUtility.AsIndex(x, y, z) * 2;
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
}