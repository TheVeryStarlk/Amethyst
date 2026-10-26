using Amethyst.Abstractions.Packets.Status;
using Amethyst.Eventing;

namespace Amethyst.Networking.Packets.Status;

internal sealed partial class PingPacket(long magic) : IIngoingPacket<PingPacket>, IProcessor
{
    public static State State => State.Status;

    public static int Identifier => 1;

    public void Process(Client client, Dispatcher dispatcher)
    {
        client.Write(new PongPacket(magic));
        client.Stop();
    }
}