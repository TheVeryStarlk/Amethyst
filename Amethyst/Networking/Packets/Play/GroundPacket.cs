using Amethyst.Eventing;

namespace Amethyst.Networking.Packets.Play;

internal sealed class GroundPacket(bool value) : IIngoingPacket<GroundPacket>, IProcessor
{
    public static int Identifier => 3;

    public static GroundPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new GroundPacket(reader.ReadBoolean());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        client.Player!.Ground = value;
    }
}