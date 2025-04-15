using Amethyst.Eventing;
using Amethyst.Utilities;

namespace Amethyst.Networking.Packets.Play;

internal sealed class StatusPacket(Status status) : IIngoingPacket<StatusPacket>, IProcessor
{
    public static int Identifier => 22;

    public static StatusPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new StatusPacket(EnumUtility.Convert<Status>(reader.ReadVariableInteger()));
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
    }
}

// Make this public with an event.
internal enum Status
{
    PerformRespawn,
    RequestStatistics,
    TakingInventoryAchievement
}