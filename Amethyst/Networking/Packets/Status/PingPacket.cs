using Amethyst.Abstractions.Networking.Packets.Status;
using Amethyst.Eventing;

namespace Amethyst.Networking.Packets.Status;

internal sealed class PingPacket(long magic) : IIngoingPacket<PingPacket>, IProcessor
{
    public static int Identifier => 1;

    public static PingPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new PingPacket(reader.ReadLong());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        client.Write(new PongPacket(magic));
        client.Stop();
    }
}