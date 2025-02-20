using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Play.Players;

public sealed record RespawnPacket(int Dimension, byte Difficulty, byte GameMode, string Type) : IOutgoingPacket
{
    public int Identifier => 7;

    public int Length => sizeof(int)
                         + sizeof(sbyte)
                         + sizeof(sbyte)
                         + Variable.GetByteCount(Type);

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger(Dimension)
            .WriteByte(Difficulty)
            .WriteByte(GameMode)
            .WriteVariableString(Type);
    }
}