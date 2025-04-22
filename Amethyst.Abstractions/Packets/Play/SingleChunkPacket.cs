namespace Amethyst.Abstractions.Packets.Play;

public sealed class SingleChunkPacket(int x, int z, byte[] sections, ushort bitmask) : IOutgoingPacket
{
    public int Identifier => 33;

    public int X => x;

    public int Z => z;

    public byte[] Sections => sections;

    public ushort Bitmask => bitmask;
}