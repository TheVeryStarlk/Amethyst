using Amethyst.Components.Entities;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play;

public sealed record PositionLookPacket(double X, double Y, double Z, float Yaw, float Pitch, bool OnGround)
    : IIngoingPacket<PositionLookPacket>, IOutgoingPacket, IPublisher
{
    public static int Identifier => 6;

    int IOutgoingPacket.Identifier => 8;

    public int Length => sizeof(double)
                         + sizeof(double)
                         + sizeof(double)
                         + sizeof(float)
                         + sizeof(float)
                         + sizeof(bool);

    public static PositionLookPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new PositionLookPacket(
            reader.ReadDouble(),
            reader.ReadDouble(),
            reader.ReadDouble(),
            reader.ReadFloat(),
            reader.ReadFloat(),
            reader.ReadBoolean());
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteDouble(X)
            .WriteDouble(Y)
            .WriteDouble(Z)
            .WriteFloat(Yaw)
            .WriteFloat(Pitch)
            .WriteBoolean(OnGround);
    }

    async Task IPublisher.PublishAsync(Packet packet, IPlayer player, EventDispatcher eventDispatcher, CancellationToken cancellationToken)
    {
        var instance = packet.Create<PositionLookPacket>();
        var moved = new Moved(instance.X, instance.Y, instance.Z, instance.Yaw, instance.Pitch, instance.OnGround);

        await eventDispatcher.DispatchAsync(player, moved, cancellationToken).ConfigureAwait(false);
    }
}