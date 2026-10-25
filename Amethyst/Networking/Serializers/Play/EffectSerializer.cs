using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class EffectSerializer(int effect, Position position, int data, bool global) : ISerializer<EffectPacket, EffectSerializer>
{
    public int Length => sizeof(int) + sizeof(long) + sizeof(int) + sizeof(bool);

    public static EffectSerializer Create(EffectPacket packet)
    {
        return new EffectSerializer(packet.Effect, packet.Position, packet.Data, packet.Global);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger(effect)
            .WritePosition(position)
            .WriteInteger(data)
            .WriteBoolean(global);
    }
}