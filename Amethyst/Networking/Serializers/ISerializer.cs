using Amethyst.Abstractions.Packets;
using Amethyst.Abstractions.Packets.Login;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Packets.Status;
using Amethyst.Networking.Serializers.Login;
using Amethyst.Networking.Serializers.Play;
using Amethyst.Networking.Serializers.Status;

namespace Amethyst.Networking.Serializers;

internal interface ISerializer
{
    public int Length { get; }

    public void Write(Span<byte> span);
}

internal interface ISerializer<in T, out TSerializer> : ISerializer where T : IOutgoingPacket where TSerializer : ISerializer
{
    public static abstract TSerializer Create(T packet);
}

internal static class OutgoingPacketExtensions
{
    public static ISerializer Create(this IOutgoingPacket instance)
    {
        // I think frozen dictionaries are faster than this.
        return instance switch
        {
            // Status.
            StatusResponsePacket packet => StatusResponseSerializer.Create(packet),
            PongPacket packet => PongSerializer.Create(packet),

            // Login.
            FailurePacket packet => FailureSerializer.Create(packet),
            SuccessPacket packet => SuccessSerializer.Create(packet),

            // Play.
            KeepAlivePacket packet => KeepAliveSerializer.Create(packet),
            JoinGamePacket packet => JoinGameSerializer.Create(packet),
            MessagePacket packet => MessageSerializer.Create(packet),
            TimePacket packet => TimeSerializer.Create(packet),
            PositionLookPacket packet => PositionLookSerializer.Create(packet),
            SpawnPlayerPacket packet => SpawnPlayerSerializer.Create(packet),
            DestroyEntitiesPacket packet => DestroyEntitiesSerializer.Create(packet),
            EntityRelativePositionLookPacket packet => EntityRelativePositionLookSerializer.Create(packet),
            SingleChunkPacket packet => SingleChunkSerializer.Create(packet),
            BlockPacket packet => BlockSerializer.Create(packet),
            ChangeStatePacket packet => ChangeStateSerializer.Create(packet),
            StatisticsPacket packet => StatisticSerializer.Create(packet),
            ListItemPacket packet => ListItemSerializer.Create(packet),
            TabResponsePacket packet => TabResponseSerializer.Create(packet),
            DisconnectPacket packet => DisconnectSerializer.Create(packet),
            ListHeaderFooterPacket packet => ListHeaderFooterSerializer.Create(packet),
            _ => throw new ArgumentOutOfRangeException(nameof(instance), instance, "Unknown packet.")
        };
    }
}