using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Networking.Ingoing.Packets.Play;

internal sealed class DiggingPacket(Digging digging, Position position, BlockFace face) : IIngoingPacket<DiggingPacket>
{
    public const int Identifier = 7;

    public Digging Digging => digging;

    public Position Position => position;

    public BlockFace Face => face;

    public static DiggingPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new DiggingPacket((Digging) reader.ReadByte(), reader.ReadPosition(), (BlockFace) reader.ReadByte());
    }
}