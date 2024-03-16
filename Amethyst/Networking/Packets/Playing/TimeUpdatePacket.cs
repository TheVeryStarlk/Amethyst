using Amethyst.Api.Levels;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class TimeUpdatePacket : IOutgoingPacket
{
    public int Identifier => 0x02;

    public required IWorld World { get; init; }

    public int CalculateLength()
    {
        return sizeof(long) * 2;
    }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteLong(World.Age);
        writer.WriteLong(World.Time);
    }
}