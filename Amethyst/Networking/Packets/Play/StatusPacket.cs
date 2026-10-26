using Amethyst.Eventing;

namespace Amethyst.Networking.Packets.Play;

internal sealed class StatusPacket(Status status) : IIngoingPacket<StatusPacket>, IProcessor
{
    public static State State => State.Play;

    public static int Identifier => 22;

    public static StatusPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new StatusPacket((Status) reader.ReadVariableInteger());
    }

    public void Process(Client client, Dispatcher dispatcher)
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