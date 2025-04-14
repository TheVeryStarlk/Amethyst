using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class DiggingPacket(Digging digging, Position position, BlockFace face) : IIngoingPacket<DiggingPacket>, IProcessor
{
    public static int Identifier => 7;

    public static DiggingPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        var digging = (Digging) reader.ReadByte();
        var value = reader.ReadLong();
        var face = (BlockFace) reader.ReadByte();

        return new DiggingPacket(
            digging,
            new Position((int) (value >> 38), (int) (value >> 26) & 0xFFF, (int) (value << 38 >> 38)),
            face);
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        client.Player!.World[(int) position.X, (int) position.Z, (int) position.Y] = Blocks.Air;
        eventDispatcher.Dispatch(client.Player, new Dig(digging, position, face));
    }
}