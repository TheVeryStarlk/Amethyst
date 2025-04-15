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
        return new DiggingPacket((Digging) reader.ReadByte(), reader.ReadPosition(), (BlockFace) reader.ReadByte());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        client.Player!.World[(int) position.X, (int) position.Y, (int) position.Z] = Blocks.Air;
        eventDispatcher.Dispatch(client.Player, new Dig(digging, position, face));
    }
}