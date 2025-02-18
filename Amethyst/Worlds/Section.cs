using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class Section
{
    private readonly byte[] blocks = new byte[8192];
    private readonly byte[] blocksLight = new byte[2048];
    private readonly byte[] skyLight = new byte[2048];

    public Block GetBlock(Position position)
    {
        var index = position.AsIndex() * 2;

        var type = blocks[index] >> 4 | blocks[index + 1] << 4;
        var metadata = blocks[index] & 0x0F;

        return new Block(type, metadata);
    }

    public void SetBlock(Block block, Position position)
    {
        var index = position.AsIndex() * 2;
        var type = block.Type;

        blocks[index] = (byte) (type << 4 | block.Metadata);
        blocks[index + 1] = (byte) (type >> 4);
    }

    public byte GetSkyLight(Position position)
    {
        return skyLight[position.AsIndex()];
    }

    public void SetSkyLight(byte value, Position position)
    {
        var index = position.AsIndex();
        skyLight[index] = value;
    }

    public (byte[] Blocks, byte[] BlocksLight, byte[] SkyLight) Build()
    {
        for (var index = 0; index < 2048; index++)
        {
            blocksLight[index] = 0xFF;
            skyLight[index] = 0xFF;
        }

        return (blocks, blocksLight, skyLight);
    }
}