using Amethyst.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class LookPacket(float yaw, float pitch, bool ground) : IIngoingPacket<LookPacket>, IProcessor
{
    public static int Identifier => 5;

    public static LookPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new LookPacket(reader.ReadFloat(), reader.ReadFloat(), reader.ReadBoolean());
    }

    public void Process(Client client)
    {
        client.EventDispatcher.Dispatch(client.Player!, new Moved(client.Player!.Position, yaw, pitch));

        client.Player.Yaw = yaw;
        client.Player.Pitch = pitch;
        client.Player.Ground = ground;
    }
}