namespace Amethyst.Networking.Ingoing.Packets.Play;

internal sealed class StatusPacket(Status status) : IIngoingPacket<StatusPacket>
{
    public const int Identifier = 22;

    public Status Status => status;

    public static StatusPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new StatusPacket((Status) reader.ReadVariableInteger());
    }
}

// Make this public with an event.
internal enum Status
{
    PerformRespawn,
    RequestStatistics,
    TakingInventoryAchievement
}