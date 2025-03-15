using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class JoinGameSerializer(JoinGamePacket packet) : Serializer(packet)
{
    public override int Identifier => 1;

    public override int Length => sizeof(int) + sizeof(byte) + sizeof(sbyte) + sizeof(byte) + sizeof(byte) + Variable.GetByteCount(type) + sizeof(bool);

    private readonly string type = packet.Type.ToString();

    public override void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger(packet.Entity)
            .WriteByte((byte) packet.GameMode)
            .WriteByte((byte) packet.Dimension)
            .WriteByte((byte) packet.Difficulty)
            .WriteByte(packet.Players)
            .WriteVariableString(type)
            .WriteBoolean(packet.ReducedDebugInformation);
    }
}