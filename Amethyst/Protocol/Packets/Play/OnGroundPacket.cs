using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Play;

public sealed record OnGroundPacket(bool Value) : IIngoingPacket<OnGroundPacket>
{
    public static int Identifier => 3;

    public static OnGroundPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new OnGroundPacket(reader.ReadBoolean());
    }
}