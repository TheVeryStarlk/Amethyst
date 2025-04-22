using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class ChangeStateSerializer(IState state) : ISerializer<ChangeStatePacket, ChangeStateSerializer>
{
    public int Length => sizeof(byte) + sizeof(float);

    public static ChangeStateSerializer Create(ChangeStatePacket packet)
    {
        return new ChangeStateSerializer(packet.State);
    }

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter.Create(span).WriteByte(state.Identifier);

        if (state is IValueState value)
        {
            writer.WriteFloat(value.Value);
        }
    }
}