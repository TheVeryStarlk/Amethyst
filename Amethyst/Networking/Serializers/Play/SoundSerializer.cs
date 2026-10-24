using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class SoundSerializer(string name, int x, int y, int z, float volume, byte pitch) : ISerializer<SoundPacket, SoundSerializer>
{
    public int Length => Variable.GetByteCount(name) + sizeof(int) * 3 + sizeof(float) + sizeof(byte);

    public static SoundSerializer Create(SoundPacket packet)
    {
        // Positions are fixed-point with 3 fractional bits, pitches are scaled so 63 is normal.
        return new SoundSerializer(
            packet.Name,
            (int) (packet.Position.X * 8D),
            (int) (packet.Position.Y * 8D),
            (int) (packet.Position.Z * 8D),
            packet.Volume,
            (byte) Math.Clamp(packet.Pitch * 63F, byte.MinValue, byte.MaxValue));
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableString(name)
            .WriteInteger(x)
            .WriteInteger(y)
            .WriteInteger(z)
            .WriteFloat(volume)
            .WriteByte(pitch);
    }
}