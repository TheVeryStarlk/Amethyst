using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Status;
using Amethyst.Networking.Serializers.Status;

namespace Amethyst.Networking.Serializers;

internal interface ISerializer
{
    public int Length { get; }

    public void Write(Span<byte> span);
}

internal interface ISerializer<in T> : ISerializer where T : IOutgoingPacket
{
    public static abstract ISerializer Create(T packet);
}

internal static class OutgoingPacketExtensions
{
    public static ISerializer Create(this IOutgoingPacket packet)
    {
        // I think frozen dictionaries are faster than this.
        return packet switch
        {
            StatusResponsePacket statusPacket => StatusResponseSerializer.Create(statusPacket),
            PongPacket pongPacket => PongSerializer.Create(pongPacket),
            _ => throw new ArgumentOutOfRangeException(nameof(packet), packet, "Unknown packet.")
        };
    }
}