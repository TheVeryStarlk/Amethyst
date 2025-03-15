using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Abstractions.Networking.Packets.Status;
using Amethyst.Networking.Serializers.Play;
using Amethyst.Networking.Serializers.Status;

namespace Amethyst.Networking.Serializers;

internal abstract class Serializer(IOutgoingPacket packet)
{
    public abstract int Identifier { get; }

    public abstract int Length { get; }

    public abstract void Write(Span<byte> span);
}

internal static class OutgoingPacketExtensions
{
    public static Serializer Create<T>(this T packet) where T : IOutgoingPacket
    {
        return packet switch
        {
            MessagePacket messagePacket => new MessageSerializer(messagePacket),
            PongPacket pongPacket => new PongSerializer(pongPacket),
            _ => throw new ArgumentOutOfRangeException(nameof(packet), packet, "Unknown packet.")
        };
    }
}