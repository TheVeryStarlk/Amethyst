using Amethyst.Api.Components;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class SoundEffectPacket : IOutgoingPacket
{
    public int Identifier => 0x29;

    public required string Effect { get; init; }

    public required Position Position { get; init; }

    public float Volume { get; init; } = 1;

    public byte Pitch { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(Effect);
        writer.WriteInteger((int) Position.X * 8);
        writer.WriteInteger((int) Position.Y * 8);
        writer.WriteInteger((int) Position.Z * 8);
        writer.WriteFloat(Volume);
        writer.WriteByte((byte) (Pitch * 63));
    }
}