using Amethyst.Api.Levels;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class TimeUpdatePacket : IOutgoingPacket
{
    public int Identifier => 0x03;

    public required IWorld World { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteLong(World.Age);
        writer.WriteLong(World.Time);
    }
}