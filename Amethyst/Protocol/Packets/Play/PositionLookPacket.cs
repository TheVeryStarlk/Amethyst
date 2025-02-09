using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
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

    async Task IPublisher.PublishAsync(IPlayer player, EventDispatcher eventDispatcher, CancellationToken cancellationToken)
    {
        await eventDispatcher.DispatchAsync(player, new Moved(X, Y, Z, Yaw, Pitch, OnGround), cancellationToken).ConfigureAwait(false);
    }
}