using Amethyst.Abstractions.Networking.Packets;
using Amethyst.Abstractions.Networking.Packets.Login;
using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Abstractions.Networking.Packets.Status;
using Amethyst.Networking.Serializers.Login;
using Amethyst.Networking.Serializers.Play;
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
            FailurePacket instance => FailureSerializer.Create(instance),
            SuccessPacket instance => SuccessSerializer.Create(instance),
            StatusResponsePacket instance => StatusResponseSerializer.Create(instance),
            PongPacket instance => PongSerializer.Create(instance),
            KeepAlivePacket instance => KeepAliveSerializer.Create(instance),
            JoinGamePacket instance => JoinGameSerializer.Create(instance),
            MessagePacket instance => MessageSerializer.Create(instance),
            PositionLookPacket instance => PositionLookSerializer.Create(instance),
            SingleChunkPacket instance => SingleChunkSerializer.Create(instance),
            DisconnectPacket instance => DisconnectSerializer.Create(instance),
            _ => throw new ArgumentOutOfRangeException(nameof(packet), packet, "Unknown packet.")
        };
    }
}