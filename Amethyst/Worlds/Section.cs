using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class Section
{
    private readonly byte[] blocks = new byte[8192];
    private readonly byte[] blocksLight = new byte[2048];
    private readonly byte[] skyLight = new byte[2048];

    public Block GetBlock(long x, long y, long z)
    {
        var index = AsIndex(x, y, z) * 2;

        var type = blocks[index] >> 4 | blocks[index + 1] << 4;
        var metadata = blocks[index] & 0x0F;

        return new Block(type, metadata);
    }

    public void SetBlock(Block block, long x, long y, long z)
    {
        var index = AsIndex(x, y, z) * 2;
        var type = block.Type;

        blocks[index] = (byte) (type << 4 | block.Metadata);
        blocks[index + 1] = (byte) (type >> 4);
    }

    public byte GetSkyLight(long x, long y, long z)
    {
        var index = AsIndex(x, y, z);
        return skyLight[index];
    }

    public void SetSkyLight(byte value, long x, long y, long z)
    {
        var index = AsIndex(x, y, z);
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

    private static long AsIndex(long x, long y, long z)
    {
        return (y & 0xF) << 8 | (z & 0xF) << 4 | x & 0xF;
    }
}