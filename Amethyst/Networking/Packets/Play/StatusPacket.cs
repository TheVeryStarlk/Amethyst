using Amethyst.Eventing;

namespace Amethyst.Networking.Packets.Play;

internal sealed partial class StatusPacket([Variable] Status status) : IIngoingPacket<StatusPacket>, IProcessor
{
    public static State State => State.Play;

    public static int Identifier => 22;

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